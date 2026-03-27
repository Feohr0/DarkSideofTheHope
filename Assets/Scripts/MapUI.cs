// Scripts/Map/MapUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Harita canvas'ını çizer. MapGenerator'dan node verisini alır,
/// UI butonlarını oluşturur ve bağlantı çizgilerini çizer.
/// </summary>
public class MapUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform        nodeContainer;
    [SerializeField] private Transform        lineContainer;
    [SerializeField] private GameObject       mapNodeUIPrefab;
    [SerializeField] private GameObject       lineUIPrefab;

    [Header("Layout")]
    [SerializeField] private float offsetX = 150f;
    [SerializeField] private float offsetY = 100f;

    private readonly List<MapNodeUI> _spawnedNodes = new List<MapNodeUI>();

    public void DrawMap(List<List<MapNode>> floorNodes)
    {
        ClearMap();

        // Node'ları oluştur
        foreach (var floor in floorNodes)
        {
            foreach (var node in floor)
            {
                var go     = Instantiate(mapNodeUIPrefab, nodeContainer);
                var nodeUI = go.GetComponent<MapNodeUI>();
                nodeUI.Setup(node);

                var rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(
                    node.UIPosition.x + offsetX,
                    node.UIPosition.y + offsetY);

                _spawnedNodes.Add(nodeUI);
            }
        }

        // Bağlantı çizgilerini çiz
        foreach (var floor in floorNodes)
        {
            foreach (var node in floor)
            {
                foreach (var next in node.NextNodes)
                {
                    DrawLine(node.UIPosition, next.UIPosition);
                }
            }
        }
    }

    public void RefreshMap(List<List<MapNode>> floorNodes)
    {
        foreach (var nodeUI in _spawnedNodes)
            nodeUI.RefreshVisual();
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        var go   = Instantiate(lineUIPrefab, lineContainer);
        var line = go.GetComponent<MapLineUI>();
        line.Setup(
            from + new Vector2(offsetX, offsetY),
            to   + new Vector2(offsetX, offsetY));
    }

    private void ClearMap()
    {
        foreach (Transform child in nodeContainer) Destroy(child.gameObject);
        foreach (Transform child in lineContainer) Destroy(child.gameObject);
        _spawnedNodes.Clear();
    }
}