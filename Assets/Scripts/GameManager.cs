using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Paneller (Canvas)")]
    public GameObject mapCanvas;     // Harita UI'ı
    public GameObject battleCanvas;  // Savaş HUD ve El UI'ı

    [Header("Bağımlılıklar")]
    public TurnManager turnManager;
    public DeckData playerMainDeck;  // Oyuncunun asıl destesi
    
    [Header("Kalıcı Oyuncu Verileri")]
    public int playerMaxHP = 30;
    public int playerCurrentHP;
    
    [Header("Ekonomi")]
    public int playerGold = 0;

    public TextMeshProUGUI goldText;
    
    public List<Card> playerCurrentDeck = new List<Card>();
    
    [Header("Harita İlerlemesi")]
    public MapNode currentNode; // O an bulunduğumuz düğüm

    [Header("Harita Mesajı (İksir vb.)")]
    public MapMessageView mapMessageView;
    
    public void AddGold(int amount)
    {
        playerGold += amount;
        Debug.Log("Coin Kazandın! Toplam: " + playerGold);
    }
    
    private void RefreshGoldText()
    {
        if (goldText != null) goldText.text = $"💰 {playerGold}";
    }
    
    // Potion Shop Fonksiyonu
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
    
    void Start()
    {
        playerCurrentHP = playerMaxHP;
        
        // Oyun ilk başladığında temel destedeki kartları (CardData), güncel destemize (Card) çevirip ekliyoruz
        foreach (CardData cardData in playerMainDeck.cards)
        {
            playerCurrentDeck.Add(cardData.ToCard());
        }
        
        // Oyun başladığında Haritayı aç, Savaşı gizle
        ShowMap();
    }

    public void ShowMap()
    {
        battleCanvas.SetActive(false);
        mapCanvas.SetActive(true);
    }
    
    public void StartEncounter(EncounterData encounterData)
    {
        // 1. Haritayı kapat, Savaşı aç
        mapCanvas.SetActive(false);
        battleCanvas.SetActive(true);

        turnManager.InitBattle(playerMainDeck, encounterData, playerMaxHP, playerCurrentHP);
    }

    public void EndBattle(bool playerWon)
    {
        // Savaş biter bitmez oyuncunun kalan canını kaydet
        playerCurrentHP = turnManager.player.health;

        turnManager.ClearBattlefield();

        // Kaybettiysek: run bitti → progress sıfırla ve menüye dön
        if (!playerWon)
        {
            ResetProgress();
            SceneManager.LoadScene(0);
            return;
        }

        // Kazandıysak: sadece boss düğümünden sonra run biter
        if (currentNode != null && currentNode.isBoss)
        {
            ResetProgress();
            SceneManager.LoadScene(0);
            return;
        }

        // Normal savaş kazanıldı → haritaya dön, ilerlemeyi aç
        ShowMap();
        CompleteCurrentNode();
    }

    private void ResetProgress()
    {
        Time.timeScale = 1f;
        
        playerGold = 0;
        RefreshGoldText();
        
        playerCurrentHP = playerMaxHP;

        currentNode = null;
        
        playerCurrentDeck.Clear();
        if (playerMainDeck != null && playerMainDeck.cards != null)
        {
            foreach (CardData cardData in playerMainDeck.cards)
            {
                playerCurrentDeck.Add(cardData.ToCard());
            }
        }
    }
    
    public void UpgradeCardType(CardData cardType, int cost)
    {
        if (playerGold >= cost && cardType.currentLevel < 3)
        {
            playerGold -= cost;
            cardType.Upgrade();
        
            // UI'ı güncellemek için Refresh çağırılabilir
            FindObjectOfType<UIManager>()?.Refresh();
            Debug.Log($"{cardType.cardName} türü Seviye {cardType.currentLevel}'e yükseltildi!");
        }
    }
    
    public void CompleteCurrentNode()
    {
        // 1. Önce haritadaki TÜM düğümleri kilitliyoruz
        MapNode[] allNodes = FindObjectsOfType<MapNode>();
        foreach (MapNode node in allNodes)
        {
            node.isUnlocked = false;
            node.nodeButton.interactable = false;
        }

        // 2. Sadece bulunduğumuz düğümün (currentNode) bağlı olduğu düğümleri açıyoruz!
        if (currentNode != null)
        {
            foreach (MapNode next in currentNode.nextNodes)
            {
                next.UnlockNode();
            }
        }
    }
    
    
}