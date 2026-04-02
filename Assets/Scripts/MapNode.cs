using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class MapNode : MonoBehaviour
{
    public enum NodeType { Battle, Shop, UpgradeShop, Random }
    
    [Header("Düğüm Ayarları")]
    public NodeType type;
    public bool isBoss; // Eğer bu bir boss savaşıysa işaretle
    public EncounterData encounter; // Savaş düğümleri için

    [Header("Görsel Ayarlar")]
    public Image iconImage; // Düğümün üzerindeki ikon objesi
    public Sprite battleSprite;
    public Sprite bossSprite;
    public Sprite shopSprite;
    public Sprite upgradeSprite;
    public Sprite randomSprite; // Genelde "?" ikonu

    [Header("Yol Bağlantıları")]
    public List<MapNode> nextNodes; // Bu düğümden sonra gidilebilecek düğümler
    public bool isUnlocked = false; // Tıklanabilir mi?
    public Button nodeButton;
    
    private void Start()
    {
        UpdateNodeVisual();
        
        // Düğüm kilitliyse butonu kapat
        if (nodeButton != null)
            nodeButton.interactable = isUnlocked;
    }

    public void UnlockNode()
    {
        isUnlocked = true;
        if (nodeButton != null) nodeButton.interactable = true;
    }
    
    // Başlangıçta türüne göre doğru ikonu atar
    public void UpdateNodeVisual()
    {
        if (iconImage == null) return;

        switch (type)
        {
            case NodeType.Battle:
                iconImage.sprite = isBoss ? bossSprite : battleSprite;
                break;
            case NodeType.Shop:
                iconImage.sprite = shopSprite;
                break;
            case NodeType.UpgradeShop:
                iconImage.sprite = upgradeSprite;
                break;
            case NodeType.Random:
                iconImage.sprite = randomSprite;
                break;
        }
    }

    public void OnNodeClicked()
    {
        if (!isUnlocked) return; // Kilitliyse hiçbir şey yapma
        
        GameManager gm = FindObjectOfType<GameManager>();
        NodeType activeType = type;
        
        gm.currentNode = this;
        
        // Eğer düğüm "Random" ise, tıklandığı an rastgele bir türe dönüşür
        if (type == NodeType.Random)
        {
            activeType = GetRandomType();
            Debug.Log($"Gizemli düğümden çıkan: {activeType}");
        }

        ExecuteNodeLogic(gm, activeType);
    }

    private NodeType GetRandomType()
    {
        // Random (3) hariç diğer 3 türden (0, 1, 2) birini seç
        int randomIndex = Random.Range(0, 3); 
        return (NodeType)randomIndex;
    }

    private void ExecuteNodeLogic(GameManager gm, NodeType activeType)
    {
        switch (activeType)
        {
            case NodeType.Battle:
                gm.StartEncounter(encounter);
                break;
            case NodeType.Shop:
                // Basitçe tıklandığında alışveriş yap (Veya Shop UI aç)
                gm.TryBuyHealth(10, 10);
                break;
            case NodeType.UpgradeShop:
                // UpgradeShopView'ı bul ve aç
                FindObjectOfType<UpgradeShopView>(true).OpenShop();
                break;
        }
    }
}