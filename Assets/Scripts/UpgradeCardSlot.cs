using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardSlot : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI priceText;
    public Button upgradeButton;

    // UpgradeShopView tarafından doldurulur
    public void Setup(CardData data, int goldCost, System.Action onUpgradeClick)
    {
        nameText.text = $"{data.cardName} (Sv. {data.currentLevel})";
        
        // O anki değerler
        int currentPower = data.GetCurrentPower();
        int currentEnergy = data.GetCurrentCost();
        
        // Bir sonraki seviyenin değerlerini görmek için geçici olarak seviyeyi artırıp okuyoruz
        data.currentLevel++;
        int nextPower = data.GetCurrentPower();
        int nextEnergy = data.GetCurrentCost();
        data.currentLevel--; // Değeri hemen geri alıyoruz ki asıl veriyi bozmayalım

        statsText.text = $"Güç: {currentPower} ➔ {nextPower}\nEnerji: {currentEnergy} ➔ {nextEnergy}";
        priceText.text = $"💰 {goldCost}";

        // Butona tıklama olayını bağla
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => onUpgradeClick?.Invoke());
    }
}