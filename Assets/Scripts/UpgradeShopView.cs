using UnityEngine;
using TMPro;

public class UpgradeShopView : MonoBehaviour
{
    [Header("Kart Verileri (Inspector'dan Sürükle)")]
    public CardData attackCard; // Saldırı kartının Asset'ini buraya sürükle
    public CardData shieldCard; // Kalkan kartının Asset'ini buraya sürükle

    [Header("Fiyatlandırma")]
    public int upgradeCost = 50; // Her geliştirmenin bedeli
    
    [Header("UI")]
    public TextMeshProUGUI goldText;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Dükkan açıldığında parayı güncelle
    private void OnEnable()
    {
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        RefreshGoldText();
    }

    private void RefreshGoldText()
    {
        if (goldText != null) goldText.text = $"💰 {gameManager.playerGold}";
    }

    // --- 1. BUTON: Saldırı Hasarı +1 ---
    public void BuyAttackDamage()
    {
        if (gameManager.playerGold >= upgradeCost)
        {
            gameManager.playerGold -= upgradeCost;
            
            // Kartın tüm seviyelerindeki gücünü 1 artırıyoruz
            for (int i = 0; i < attackCard.powerLevels.Length; i++)
                attackCard.powerLevels[i] += 1;

            Debug.Log("Saldırı Kartlarının Hasarı +1 Arttı!");
            RefreshGoldText();
        }
        else Debug.Log("Altın yetersiz!");
        CloseShop();
    }

    // --- 2. BUTON: Saldırı Enerjisi -1 ---
    public void BuyAttackCostReduction()
    {
        // Enerji zaten 0 ise daha fazla düşemesin diye kontrol ekliyoruz
        if (gameManager.playerGold >= upgradeCost && attackCard.GetCurrentCost() > 0)
        {
            gameManager.playerGold -= upgradeCost;
            
            for (int i = 0; i < attackCard.energyLevels.Length; i++)
                attackCard.energyLevels[i] = Mathf.Max(0, attackCard.energyLevels[i] - 1);

            Debug.Log("Saldırı Kartlarının Maliyeti -1 Düştü!");
            RefreshGoldText();
        }
        else Debug.Log("Altın yetersiz veya maliyet zaten 0!");
        CloseShop();
    }

    // --- 3. BUTON: Kalkan Gücü +1 ---
    public void BuyShieldPower()
    {
        if (gameManager.playerGold >= upgradeCost)
        {
            gameManager.playerGold -= upgradeCost;
            
            for (int i = 0; i < shieldCard.powerLevels.Length; i++)
                shieldCard.powerLevels[i] += 1;

            Debug.Log("Kalkan Kartlarının Gücü +1 Arttı!");
            RefreshGoldText();
        }
        else Debug.Log("Altın yetersiz!");
        CloseShop();
    }

    // --- 4. BUTON: Kalkan Enerjisi -1 ---
    public void BuyShieldCostReduction()
    {
        if (gameManager.playerGold >= upgradeCost && shieldCard.GetCurrentCost() > 0)
        {
            gameManager.playerGold -= upgradeCost;
            
            for (int i = 0; i < shieldCard.energyLevels.Length; i++)
                shieldCard.energyLevels[i] = Mathf.Max(0, shieldCard.energyLevels[i] - 1);

            Debug.Log("Kalkan Kartlarının Maliyeti -1 Düştü!");
            RefreshGoldText();
        }
        else Debug.Log("Altın yetersiz veya maliyet zaten 0!");
        
        CloseShop();
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);
        //gameManager.CompleteCurrentNode(); // Yolu açar
    }
    
    // Haritaya geri dönmek için
    public void CloseShop()
    {
        gameObject.SetActive(false);
        gameManager.CompleteCurrentNode(); // Yolu açar
    }
}