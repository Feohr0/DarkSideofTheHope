using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Paneller (Canvas)")]
    [Tooltip("3 world'ün harita canvas'ları — sırasıyla World 1, 2, 3")]
    public GameObject[] worldMapCanvases;   // Her world'ün içindeki level seçme canvas'ı
    public GameObject mapCanvas;             // Ana level seçme sahne canvas'ı (tüm world'lerin kabı)
    public GameObject battleCanvas;

    [Header("Arkaplan")]
    public SpriteRenderer backgroundRenderer;

    [System.Serializable]
    public class WorldBackgroundSet
    {
        [Tooltip("Harita ekranındaki arkaplan")]
        public Sprite mapBg;
        [Tooltip("Normal savaş arkaplanı")]
        public Sprite battleBg;
        [Tooltip("Boss savaşı arkaplanı")]
        public Sprite bossBg;
    }

    [Tooltip("Her world için ayrı arkaplan seti (sırasıyla World 1, 2, 3)")]
    public WorldBackgroundSet[] worldBackgrounds;

    [Header("Bağımlılıklar")]
    public TurnManager turnManager;
    public DeckData playerMainDeck;

    [Header("Kalıcı Oyuncu Verileri")]
    public int playerMaxHP = 30;
    public int playerCurrentHP;

    [Header("Savaş Sonu")]
    [Tooltip("Savaş bittikten sonra haritaya dönmeden önceki bekleme süresi (saniye)")]
    [SerializeField] private float endBattleDelay = 1f;

    [Header("Ekonomi")]
    public int playerGold = 0;
    public TextMeshProUGUI goldText;

    public List<Card> playerCurrentDeck = new List<Card>();

    [Header("Harita İlerlemesi")]
    public MapNode currentNode;

    /// <summary>Aktif node'u değiştirir ve halka gösterimini günceller.</summary>
    public void SetCurrentNode(MapNode newNode)
    {
        if (currentNode != null)
            currentNode.HideRing();

        currentNode = newNode;

        if (currentNode != null)
            currentNode.ShowRing();
    }

    /// <summary>Şu an hangi world'deyiz (0 = World 1, 1 = World 2, 2 = World 3)</summary>
    [HideInInspector] public int currentWorldIndex = 0;

    [Header("Harita Mesajı (İksir vb.)")]
    public MapMessageView mapMessageView;

    public void AddGold(int amount)
    {
        playerGold += amount;
        RefreshGoldText();
        Debug.Log("Coin Kazandın! Toplam: " + playerGold);
    }

    public void NotifyGoldChanged()
    {
        RefreshGoldText();
    }

    private void RefreshGoldText()
    {
        if (goldText != null)
        {
            goldText.text = playerGold.ToString();
        }
    }

    public bool TryBuyHealth(int cost, int healAmount)
    {
        int beforeGold = playerGold;
        int beforeHP = playerCurrentHP;

        if (playerGold >= cost)
        {
            playerGold -= cost;
            playerCurrentHP = Mathf.Min(playerCurrentHP + healAmount, playerMaxHP);
            Debug.Log($"İksir alındı! Kalan Altın: {playerGold} | Yeni HP: {playerCurrentHP}");
            RefreshGoldText();

            int healed = playerCurrentHP - beforeHP;
            if (healed <= 0)
            {
                ShowMapMessage($"Canın zaten full: {playerCurrentHP}/{playerMaxHP} | Ruh Puanı: {playerGold}");
            }
            else
            {
                ShowMapMessage($"+{healed} can kazandın ({playerCurrentHP}/{playerMaxHP}) | Ruh Puanı: {playerGold}");
            }
            return true;
        }

        Debug.Log("Yetersiz altın!");
        ShowMapMessage($"Yetersiz Ruh Puanı: {beforeGold}/{cost} | Can: {beforeHP}/{playerMaxHP}");
        return false;
    }

    public bool TryUpgradeCard(CardData cardType, int cost)
    {
        if (cardType == null)
        {
            return false;
        }

        if (playerGold < cost)
        {
            ShowMapMessage($"Yetersiz Ruh Puanı: {playerGold}/{cost}");
            return false;
        }

        if (!cardType.CanUpgrade())
        {
            ShowMapMessage($"{cardType.cardName} zaten maksimum seviyede.");
            return false;
        }

        playerGold -= cost;
        cardType.Upgrade();
        RefreshGoldText();

        FindObjectOfType<UIManager>()?.Refresh();
        ShowMapMessage($"{cardType.cardName} seviye {cardType.currentLevel} oldu. Kalan Ruh Puanı: {playerGold}");
        Debug.Log($"{cardType.cardName} türü Seviye {cardType.currentLevel}'e yükseltildi!");
        return true;
    }

    public void UpgradeCardType(CardData cardType, int cost)
    {
        TryUpgradeCard(cardType, cost);
    }

    private void ShowMapMessage(string message)
    {
        if (mapMessageView != null)
        {
            mapMessageView.Show(message);
            return;
        }

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null && ui.hudView != null)
        {
            ui.hudView.ShowLog(message);
            return;
        }

        Debug.Log(message);
    }

    private void Start()
    {
        playerCurrentHP = playerMaxHP;

        foreach (CardData cardData in playerMainDeck.cards)
        {
            playerCurrentDeck.Add(cardData.ToCard());
        }

        RefreshGoldText();
        ShowMap();
    }

    // --------------- Harita Yönetimi ---------------

    /// <summary>Ana map canvas'ı ve tüm world canvas'larını kapat.</summary>
    public void HideAllWorldMaps()
    {
        // Ana level seçme canvas'nı kapat
        if (mapCanvas != null) mapCanvas.SetActive(false);

        // World'e özgü canvas'ları kapat
        if (worldMapCanvases == null) return;
        foreach (GameObject canvas in worldMapCanvases)
            if (canvas != null) canvas.SetActive(false);
    }

    /// <summary>Mevcut world'e ait harita canvas'nı aç, battle'u kapat.</summary>
    public void ShowMap()
    {
        battleCanvas.SetActive(false);
        HideAllWorldMaps();

        // Ana level seçme canvas'nı aç
        if (mapCanvas != null) mapCanvas.SetActive(true);

        // Aktif world'ün canvas'ını aç
        if (worldMapCanvases != null &&
            currentWorldIndex >= 0 &&
            currentWorldIndex < worldMapCanvases.Length &&
            worldMapCanvases[currentWorldIndex] != null)
        {
            worldMapCanvases[currentWorldIndex].SetActive(true);
        }

        SetBackgroundIndex(0);
    }

    public void StartEncounter(EncounterData encounterData)
    {
        HideAllWorldMaps();
        battleCanvas.SetActive(true);

        bool isBoss = currentNode != null && currentNode.isBoss;
        SetBackgroundIndex(isBoss ? 2 : 1);

        turnManager.InitBattle(playerMainDeck, encounterData, playerMaxHP, playerCurrentHP);
    }

    public void EndBattle(bool playerWon)
    {
        playerCurrentHP = turnManager.player.health;
        cachedPlayerWon = playerWon;
        Debug.Log($"EndBattle çağrıldı → playerWon={playerWon}, {endBattleDelay}s sonra haritaya dönülecek.");
        Invoke(nameof(ExecuteEndBattle), endBattleDelay);
    }

    private bool cachedPlayerWon;

    private void ExecuteEndBattle()
    {
        Debug.Log($"ExecuteEndBattle → Haritaya dönülüyor. playerWon={cachedPlayerWon}");
        turnManager.ClearBattlefield();

        // --- Başarısızlık: her şeyi sıfırla, World 1'e dön ---
        if (!cachedPlayerWon)
        {
            ResetProgress();   // currentWorldIndex = 0 da burada yapılıyor
            ShowMap();         // World 1 canvas'nı aç
            return;
        }

        // --- Boss yenildi ---
        if (currentNode != null && currentNode.isBoss)
        {
            AdvanceToNextWorld();
            return;
        }

        // --- Normal savaş kazanıldı ---
        ShowMap();
        CompleteCurrentNode();
    }

    /// <summary>
    /// Mevcut world'ün boss'u yenilince çağrılır.
    /// Son world ise oyun kazanılmış demektir.
    /// </summary>
    private void AdvanceToNextWorld()
    {
        int totalWorlds = worldMapCanvases != null ? worldMapCanvases.Length : 0;

        if (currentWorldIndex + 1 >= totalWorlds)
        {
            // Tüm world'ler tamamlandı → oyun kazanıldı!
            Debug.Log("Tüm dünyaları tamamladın! Oyun kazanıldı!");
            // İstersen burada bir "Kazan" ekranı açabilirsin
            ResetProgress();
            ShowMap();   // World 1'e dön (ya da ayrı bir win scene)
            return;
        }

        // Sonraki world'e geç
        currentWorldIndex++;
        SetCurrentNode(null);
        Debug.Log($"World {currentWorldIndex} başlıyor!");

        // Yeni world'ün ilk node'unu aç
        UnlockFirstNodeOfCurrentWorld();
        ShowMap();
    }

    /// <summary>
    /// Aktif world canvas'ındaki tüm MapNode'ları kilitler,
    /// sonra ilk node'u (startNode / isBoss == false olan ilkı) açar.
    /// İnspector'da "World Start Node" referansı vermek istemeyenler için
    /// basit otomatik çözüm: canvas altındaki MapNode'lardan isUnlocked=true olanları aç.
    /// </summary>
    private void UnlockFirstNodeOfCurrentWorld()
    {
        if (worldMapCanvases == null ||
            currentWorldIndex >= worldMapCanvases.Length ||
            worldMapCanvases[currentWorldIndex] == null) return;

        MapNode[] nodes = worldMapCanvases[currentWorldIndex]
                            .GetComponentsInChildren<MapNode>(true);

        // Önce hepsini kilitle
        foreach (MapNode node in nodes)
        {
            node.isUnlocked = false;
            if (node.nodeButton != null) node.nodeButton.interactable = false;
        }

        // Başlangıç node'u: isBoss değil + başka bir node'un nextNodes'unda OLMAYAN
        // (basit: sadece ChildOrder=0 ya da "startNode" flag'i — şimdilik
        //  dışarıdan referans olarak "worldStartNodes" array'i kullanıyoruz)
        if (worldStartNodes != null &&
            currentWorldIndex < worldStartNodes.Length &&
            worldStartNodes[currentWorldIndex] != null)
        {
            worldStartNodes[currentWorldIndex].UnlockNode();
        }
    }

    /// <summary>
    /// slot: 0 = harita, 1 = normal savaş, 2 = boss savaşı
    /// World, currentWorldIndex'ünden otomatik alınır.
    /// </summary>
    private void SetBackgroundIndex(int slot)
    {
        if (backgroundRenderer == null) return;
        if (worldBackgrounds == null || worldBackgrounds.Length == 0) return;

        int wi = Mathf.Clamp(currentWorldIndex, 0, worldBackgrounds.Length - 1);
        WorldBackgroundSet set = worldBackgrounds[wi];
        if (set == null) return;

        Sprite chosen = slot switch
        {
            1 => set.battleBg,
            2 => set.bossBg,
            _ => set.mapBg      // 0 veya diğer → harita arkaplanı
        };

        if (chosen != null)
            backgroundRenderer.sprite = chosen;
    }

    private void ResetProgress()
    {
        Time.timeScale = 1f;

        // World sıfırla
        currentWorldIndex = 0;

        playerGold = 0;
        RefreshGoldText();

        playerCurrentHP = playerMaxHP;
        SetCurrentNode(null);

        playerCurrentDeck.Clear();
        if (playerMainDeck != null && playerMainDeck.cards != null)
        {
            foreach (CardData cardData in playerMainDeck.cards)
            {
                playerCurrentDeck.Add(cardData.ToCard());
            }
        }

        // World 1'in başlangıç node'unu tekrar aç
        UnlockFirstNodeOfCurrentWorld();
    }

    public void CompleteCurrentNode()
    {
        // Sadece aktif world'deki node'ları kilitle
        if (worldMapCanvases != null &&
            currentWorldIndex < worldMapCanvases.Length &&
            worldMapCanvases[currentWorldIndex] != null)
        {
            MapNode[] worldNodes = worldMapCanvases[currentWorldIndex]
                                       .GetComponentsInChildren<MapNode>(true);
            foreach (MapNode node in worldNodes)
            {
                node.isUnlocked = false;
                if (node.nodeButton != null) node.nodeButton.interactable = false;
            }
        }

        if (currentNode != null)
        {
            foreach (MapNode next in currentNode.nextNodes)
                next.UnlockNode();
        }
    }

    // --------------- Inspector referansları ---------------

    [Header("World Başlangıç Node'ları")]
    [Tooltip("Her world'ün ilk açılması gereken MapNode'u — index world'e karşılık gelir")]
    public MapNode[] worldStartNodes;   // World 1 baş, World 2 baş, World 3 baş
}
