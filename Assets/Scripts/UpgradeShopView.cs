using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeShopView : MonoBehaviour
{
    [Header("UI Bağlantıları")]
    public Transform listContainer;        // Kartların dizileceği Layout Group
    public GameObject upgradeSlotPrefab;   // Üzerinde UpgradeCardSlot.cs olan Prefab
    public TextMeshProUGUI shopGoldText;   // Mağazadaki paramız
    public Button closeButton;             // Mağazadan çıkış butonu

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
    }

    // Harita düğümünden (MapNode) çağrılır
    public void OpenShop()
    {
        gameObject.SetActive(true);
        RefreshShop();
    }

    private void RefreshShop()
    {
        // 1. Paramızı güncelle
        if (shopGoldText != null) 
            shopGoldText.text = $"💰 {gameManager.playerGold}";

        // 2. Önceki açılıştan kalan eski kartları temizle
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }

        // 3. Oyuncunun destesindeki BENZERSİZ kart türlerini (CardData) bul
        List<CardData> uniqueTypes = new List<CardData>();
        foreach (Card card in gameManager.playerCurrentDeck)
        {
            if (!uniqueTypes.Contains(card.data))
            {
                uniqueTypes.Add(card.data);
            }
        }

        // 4. Ekranda listele
        foreach (CardData type in uniqueTypes)
        {
            // Sadece max seviyeye (3) ulaşmamış kartları göster
            if (type.currentLevel < 3)
            {
                GameObject go = Instantiate(upgradeSlotPrefab, listContainer);
                UpgradeCardSlot slot = go.GetComponent<UpgradeCardSlot>();
                
                // Formül: Seviye 0 -> 50 altın, Seviye 1 -> 100 altın, Seviye 2 -> 150 altın
                int upgradeCost = 50 * (type.currentLevel + 1);

                slot.Setup(type, upgradeCost, () => OnUpgradeButtonClicked(type, upgradeCost));
            }
        }
    }

    private void OnUpgradeButtonClicked(CardData type, int cost)
    {
        if (gameManager.playerGold >= cost)
        {
            // GameManager üzerinden parayı düş ve kartı geliştir
            gameManager.UpgradeCardType(type, cost);
            
            // Satın alımdan sonra listeyi ve altın miktarını yenile
            RefreshShop(); 
        }
        else
        {
            Debug.Log("Altın yetersiz!");
        }
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        // İsteğe bağlı olarak GameManager'a "ben işimi bitirdim, haritaya dön" diyebilirsin
        // gameManager.ShowMap(); 
    }
}