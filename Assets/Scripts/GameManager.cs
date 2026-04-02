using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
        if (playerGold >= cost)
        {
            playerGold -= cost;
            playerCurrentHP = Mathf.Min(playerCurrentHP + healAmount, playerMaxHP);
            Debug.Log($"İksir alındı! Kalan Altın: {playerGold} | Yeni HP: {playerCurrentHP}");
            RefreshGoldText();
            return true;
        }
        
        Debug.Log("Yetersiz altın!");
        return false;
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
        ShowMap();

        if (playerCurrentHP <= 0)
        {
            Debug.Log("Öldün! Oyun baştan başlıyor...");
            playerCurrentHP = playerMaxHP; // Test için resetleyebilirsin
        }
        
        if (playerWon)
        {
            CompleteCurrentNode(); // YENİ EKLENEN SATIR
        }
        else
        {
            // UI'daki log kısmına neden kaybettiğini yaz
            FindObjectOfType<UIManager>().hudView.ShowLog("DESTE TÜKENDİ! ELENDİN.");
            Debug.Log("Yenilgi!");
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