using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Paneller (Canvas)")]
    public GameObject mapCanvas;
    public GameObject battleCanvas;

    [Header("Arkaplan")]
    public SpriteRenderer backgroundRenderer;
    public Sprite[] backgroundSprites;

    [Header("Bağımlılıklar")]
    public TurnManager turnManager;
    public DeckData playerMainDeck;

    [Header("Kalıcı Oyuncu Verileri")]
    public int playerMaxHP = 30;
    public int playerCurrentHP;

    [Header("Ekonomi")]
    public int playerGold = 0;
    public TextMeshProUGUI goldText;

    public List<Card> playerCurrentDeck = new List<Card>();

    [Header("Harita İlerlemesi")]
    public MapNode currentNode;

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

    public void ShowMap()
    {
        battleCanvas.SetActive(false);
        mapCanvas.SetActive(true);
        SetBackgroundIndex(0);
    }

    public void StartEncounter(EncounterData encounterData)
    {
        mapCanvas.SetActive(false);
        battleCanvas.SetActive(true);

        bool isBoss = currentNode != null && currentNode.isBoss;
        SetBackgroundIndex(isBoss ? 2 : 1);

        turnManager.InitBattle(playerMainDeck, encounterData, playerMaxHP, playerCurrentHP);
    }

    public void EndBattle(bool playerWon)
    {
        playerCurrentHP = turnManager.player.health;

        turnManager.ClearBattlefield();

        if (!playerWon)
        {
            ResetProgress();
            SceneManager.LoadScene(0);
            return;
        }

        if (currentNode != null && currentNode.isBoss)
        {
            ResetProgress();
            SceneManager.LoadScene(0);
            return;
        }

        ShowMap();
        CompleteCurrentNode();
    }

    private void SetBackgroundIndex(int index)
    {
        if (backgroundRenderer == null) return;
        if (backgroundSprites == null || backgroundSprites.Length == 0) return;
        if (index < 0 || index >= backgroundSprites.Length) return;
        if (backgroundSprites[index] == null) return;

        backgroundRenderer.sprite = backgroundSprites[index];
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

    public void CompleteCurrentNode()
    {
        MapNode[] allNodes = FindObjectsOfType<MapNode>();
        foreach (MapNode node in allNodes)
        {
            node.isUnlocked = false;
            node.nodeButton.interactable = false;
        }

        if (currentNode != null)
        {
            foreach (MapNode next in currentNode.nextNodes)
            {
                next.UnlockNode();
            }
        }
    }
}
