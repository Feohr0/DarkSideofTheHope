using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapNode : MonoBehaviour
{
    public EncounterData encounter; // Inspector'dan o seviyenin düşmanını sürükle
    public TextMeshProUGUI nodeText;
    
    public enum NodeType { Battle, Shop }
    public NodeType type;

    public int potionCost = 50;     // Dükkan fiyatı
    public int healAmount = 10;
    
    private void Start()
    {
        if (encounter != null && nodeText != null)
        {
            nodeText.text = encounter.enemyName; // Butonun üstünde düşman adı yazsın
        }

        GetComponent<Button>().onClick.AddListener(OnNodeClicked);
    }

    private void OnNodeClicked()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if (type == NodeType.Battle)
        {
            gm.StartEncounter(encounter);
        }
        else if (type == NodeType.Shop)
        {
            // Basitçe tıklandığında alışveriş yap (İleride Shop UI açabiliriz)
            gm.TryBuyHealth(potionCost, healAmount);
        }
    }
}