using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapNode : MonoBehaviour
{
    public EncounterData encounter; // Inspector'dan o seviyenin düşmanını sürükle
    public TextMeshProUGUI nodeText;
    
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
        // GameManager'a bu karşılaşmayı başlatmasını söyleyeceğiz
        FindObjectOfType<GameManager>().StartEncounter(encounter);
    }
}