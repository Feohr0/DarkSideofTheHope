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

    [Header("Aktif Halka (Bulunduğun Node)")]
    [Tooltip("Bulunduğun düğümü belirten halka. Boş bırakırsan otomatik oluşturulur.")]
    public GameObject activeRingObject;

    [Tooltip("Halka için kullanılacak sprite (Inspector'dan ata)")]
    public Sprite ringSprite;

    [Header("Yol Bağlantıları")]
    public List<MapNode> nextNodes; // Bu düğümden sonra gidilebilecek düğümler
    public bool isUnlocked = false; // Tıklanabilir mi?
    public Button nodeButton;
    
    private void Start()
    {
        UpdateNodeVisual();
        EnsureRing();
        HideRing(); // Başlangıçta gizle
        
        // Düğüm kilitliyse butonu kapat
        if (nodeButton != null)
            nodeButton.interactable = isUnlocked;
    }

    public void UnlockNode()
    {
        isUnlocked = true;
        if (nodeButton != null) nodeButton.interactable = true;
    }

    // ─────────── Halka Göster / Gizle ───────────

    /// <summary>Bu düğümün üzerinde kırmızı halkayı göster.</summary>
    public void ShowRing()
    {
        EnsureRing();
        if (activeRingObject != null)
            activeRingObject.SetActive(true);
    }

    /// <summary>Kırmızı halkayı gizle.</summary>
    public void HideRing()
    {
        if (activeRingObject != null)
            activeRingObject.SetActive(false);
    }

    private void EnsureRing()
    {
        if (activeRingObject != null) return;

        // --- Otomatik halka oluştur ---
        activeRingObject = new GameObject("ActiveRing");
        activeRingObject.transform.SetParent(transform, false);
        activeRingObject.transform.SetAsFirstSibling(); // İkonun arkasına

        Image ringImage = activeRingObject.AddComponent<Image>();
        ringImage.raycastTarget = false;

        // Sprite atanmışsa kullan
        if (ringSprite != null)
        {
            ringImage.sprite = ringSprite;
            ringImage.type = Image.Type.Simple;
            ringImage.preserveAspect = true;
            ringImage.color = Color.white;
        }
        else
        {
            // Sprite yoksa düz kırmızı kare göster (fallback)
            ringImage.color = new Color(1f, 0.15f, 0.15f, 0.6f);
        }

        // Boyut: düğümden biraz büyük
        RectTransform rt = activeRingObject.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        RectTransform parentRT = GetComponent<RectTransform>();
        if (parentRT != null)
        {
            float size = Mathf.Max(parentRT.sizeDelta.x, parentRT.sizeDelta.y) * 1.35f;
            rt.sizeDelta = new Vector2(size, size);
        }
        else
        {
            rt.sizeDelta = new Vector2(90f, 90f);
        }

        // Nabız bileşenini ekle
        activeRingObject.AddComponent<RingPulse>();
    }

    // ─────────── Görsel ───────────
    
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
        
        gm.SetCurrentNode(this);
        
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
                gm.CompleteCurrentNode();
                break;
            case NodeType.UpgradeShop:
                // UpgradeShopView'ı bul ve aç
                FindObjectOfType<UpgradeShopView>(true).OpenShop();
                break;
        }
    }
}