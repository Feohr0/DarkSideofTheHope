// Scripts/Map/MapNodeUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Haritadaki tek bir node'un UI temsili.
/// Tıklanınca MapGenerator.OnNodeSelected çağırır.
/// </summary>
public class MapNodeUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button          button;
    [SerializeField] private Image           iconImage;
    [SerializeField] private Image           backgroundImage;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private GameObject      completedOverlay;
    [SerializeField] private GameObject      lockedOverlay;

    [Header("Node Colors")]
    [SerializeField] private Color combatColor   = new Color(0.8f, 0.2f, 0.2f);
    [SerializeField] private Color eliteColor    = new Color(0.6f, 0.1f, 0.8f);
    [SerializeField] private Color bossColor     = new Color(0.9f, 0.1f, 0.1f);
    [SerializeField] private Color restColor     = new Color(0.2f, 0.7f, 0.3f);
    [SerializeField] private Color shopColor     = new Color(0.9f, 0.8f, 0.1f);
    [SerializeField] private Color eventColor    = new Color(0.2f, 0.5f, 0.9f);
    [SerializeField] private Color treasureColor = new Color(0.9f, 0.6f, 0.1f);

    private MapNode _node;

    public void Setup(MapNode node)
    {
        _node = node;

        labelText.text         = GetNodeLabel(node.Type);
        backgroundImage.color  = GetNodeColor(node.Type);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => MapGenerator.Instance.OnNodeSelected(_node));

        RefreshVisual();
    }

    public void RefreshVisual()
    {
        if (_node == null) return;

        bool locked    = !_node.IsAccessible;
        bool completed = _node.IsCompleted;

        button.interactable = _node.IsAccessible && !_node.IsCompleted;

        if (completedOverlay != null) completedOverlay.SetActive(completed);
        if (lockedOverlay    != null) lockedOverlay.SetActive(locked);

        // Erişilemeyen node'ları soluk göster
        var group = GetComponent<CanvasGroup>();
        if (group != null)
            group.alpha = locked ? 0.4f : 1f;
    }

    private string GetNodeLabel(NodeType type)
    {
        switch (type)
        {
            case NodeType.Combat:   return "⚔";
            case NodeType.Elite:    return "☠";
            case NodeType.Boss:     return "👑";
            case NodeType.Rest:     return "🔥";
            case NodeType.Shop:     return "🛒";
            case NodeType.Event:    return "?";
            case NodeType.Treasure: return "💰";
            default:                return "?";
        }
    }

    private Color GetNodeColor(NodeType type)
    {
        switch (type)
        {
            case NodeType.Combat:   return combatColor;
            case NodeType.Elite:    return eliteColor;
            case NodeType.Boss:     return bossColor;
            case NodeType.Rest:     return restColor;
            case NodeType.Shop:     return shopColor;
            case NodeType.Event:    return eventColor;
            case NodeType.Treasure: return treasureColor;
            default:                return Color.gray;
        }
    }
}