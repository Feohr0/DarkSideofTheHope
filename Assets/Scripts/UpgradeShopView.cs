using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopView : MonoBehaviour
{
    [Header("Veri Kaynağı")]
    public DeckData playerDeck;
    public CardData[] shopCards;

    [Header("Slotlar")]
    public UpgradeCardSlot[] cardSlots;

    [Header("Fiyatlandırma")]
    public int upgradeCost = 50;

    [Header("Üst Bilgi")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI statusText;

    [Header("Sağ Panel")]
    public Image selectedCardArt;
    public TextMeshProUGUI selectedCardNameText;
    public TextMeshProUGUI selectedCardLevelText;
    public TextMeshProUGUI selectedCardCurrentStatsText;
    public TextMeshProUGUI selectedCardNextStatsText;
    public TextMeshProUGUI selectedCardCostText;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;

    private readonly List<CardData> uniqueCards = new List<CardData>();
    private GameManager gameManager;
    private CardData selectedCard;

    private void Awake()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(UpgradeSelectedCard);
        }
    }

    private void Start()
    {
        CacheReferences();
        BuildCardList();
        RefreshView();
    }

    private void OnEnable()
    {
        CacheReferences();
        BuildCardList();
        RefreshView();
    }

    public void OpenShop()
    {
        CacheReferences();
        BuildCardList();
        gameObject.SetActive(true);
        RefreshView();
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);

        if (gameManager != null)
        {
            gameManager.CompleteCurrentNode();
        }
    }

    public void SelectCard(CardData card)
    {
        if (card == null)
        {
            return;
        }

        selectedCard = card;
        RefreshView();
    }

    public void UpgradeSelectedCard()
    {
        if (selectedCard == null || gameManager == null)
        {
            return;
        }

        bool upgraded = gameManager.TryUpgradeCard(selectedCard, upgradeCost);
        statusText.text = upgraded
            ? $"{selectedCard.cardName} geliştirildi."
            : statusText.text;

        RefreshView();
    }

    // Eski sahne butonları kopmasın diye bırakıldı.
    public void BuyAttackDamage() => UpgradeSelectedCard();
    public void BuyAttackCostReduction() => UpgradeSelectedCard();
    public void BuyShieldPower() => UpgradeSelectedCard();
    public void BuyShieldCostReduction() => UpgradeSelectedCard();

    private void CacheReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (playerDeck == null && gameManager != null)
        {
            playerDeck = gameManager.playerMainDeck;
        }
    }

    private void BuildCardList()
    {
        uniqueCards.Clear();

        if (shopCards != null && shopCards.Length > 0)
        {
            foreach (CardData card in shopCards)
            {
                if (card == null)
                {
                    continue;
                }

                uniqueCards.Add(card);
            }
        }
        else if (playerDeck != null && playerDeck.cards != null)
        {
            HashSet<CardData> seenCards = new HashSet<CardData>();
            foreach (CardData card in playerDeck.cards)
            {
                if (card == null || !seenCards.Add(card))
                {
                    continue;
                }

                uniqueCards.Add(card);
            }
        }

        if (uniqueCards.Count == 0)
        {
            selectedCard = null;
            return;
        }

        if ((selectedCard == null || !uniqueCards.Contains(selectedCard)) && uniqueCards.Count > 0)
        {
            selectedCard = uniqueCards[0];
        }
    }

    private void RefreshView()
    {
        RefreshGoldText();
        RefreshSlots();
        RefreshDetails();
    }

    private void RefreshGoldText()
    {
        if (goldText != null && gameManager != null)
        {
            goldText.text = $"RUH PUANI: {gameManager.playerGold}";
        }
    }

    private void RefreshSlots()
    {
        if (cardSlots == null)
        {
            return;
        }

        for (int i = 0; i < cardSlots.Length; i++)
        {
            UpgradeCardSlot slot = cardSlots[i];
            if (slot == null)
            {
                continue;
            }

            CardData card = i < uniqueCards.Count ? uniqueCards[i] : null;
            slot.Setup(card, card == selectedCard, SelectCard);
        }
    }

    private void RefreshDetails()
    {
        bool hasSelection = selectedCard != null;

        if (selectedCardArt != null)
        {
            selectedCardArt.sprite = hasSelection ? selectedCard.cardArt : null;
            selectedCardArt.enabled = hasSelection && selectedCard.cardArt != null;
        }

        if (selectedCardNameText != null)
        {
            selectedCardNameText.text = hasSelection ? selectedCard.cardName : "Kart seç";
        }

        if (selectedCardLevelText != null)
        {
            selectedCardLevelText.text = hasSelection
                ? $"Seviye {selectedCard.currentLevel}/{CardData.MaxUpgradeLevel}"
                : string.Empty;
        }

        if (selectedCardCurrentStatsText != null)
        {
            selectedCardCurrentStatsText.text = hasSelection
                ? $"Şu an\nGüç: {selectedCard.GetCurrentPower()}\nEnerji: {selectedCard.GetCurrentCost()}"
                : string.Empty;
        }

        if (selectedCardNextStatsText != null)
        {
            selectedCardNextStatsText.text = hasSelection
                ? BuildNextStatsText(selectedCard)
                : string.Empty;
        }

        if (selectedCardCostText != null)
        {
            selectedCardCostText.text = hasSelection
                ? $"Geliştirme Bedeli: {upgradeCost} Ruh"
                : string.Empty;
        }

        if (upgradeButton != null)
        {
            bool canAfford = hasSelection && gameManager != null && gameManager.playerGold >= upgradeCost;
            bool canUpgrade = hasSelection && selectedCard.CanUpgrade();
            upgradeButton.interactable = hasSelection && canAfford && canUpgrade;
        }

        if (upgradeButtonText != null)
        {
            if (!hasSelection)
            {
                upgradeButtonText.text = "Kart Seç";
            }
            else if (!selectedCard.CanUpgrade())
            {
                upgradeButtonText.text = "Max Seviye";
            }
            else
            {
                upgradeButtonText.text = "Güçlendir";
            }
        }

        if (statusText != null)
        {
            statusText.text = BuildStatusText();
        }
    }

    private string BuildNextStatsText(CardData card)
    {
        if (!card.CanUpgrade())
        {
            return "Sonraki Seviye\nMaksimum seviyeye ulaştı.";
        }

        int nextLevel = card.currentLevel + 1;
        return $"Sonraki Seviye\nGüç: {card.GetCurrentPower()} -> {card.GetPowerAtLevel(nextLevel)}\nEnerji: {card.GetCurrentCost()} -> {card.GetCostAtLevel(nextLevel)}";
    }

    private string BuildStatusText()
    {
        if (selectedCard == null)
        {
            return "Soldan bir kart seç.";
        }

        if (!selectedCard.CanUpgrade())
        {
            return "Bu kart zaten maksimum seviyede.";
        }

        if (gameManager == null)
        {
            return string.Empty;
        }

        if (gameManager.playerGold < upgradeCost)
        {
            return $"Yetersiz Ruh Puanı: {gameManager.playerGold}/{upgradeCost}";
        }

        return $"{selectedCard.cardName} geliştirilmeye hazır.";
    }
}
