// Scripts/Map/MapGenerator.cs
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Combat, Elite, Boss, Rest, Shop, Event, Treasure }

public class MapNode
{
    public NodeType Type;
    public int      Floor;
    public int      Column;
    public bool     IsCompleted;
    public bool     IsAccessible;
    public List<MapNode> NextNodes = new List<MapNode>();
    public Vector2  UIPosition;
}

/// <summary>
/// Slay the Spire tarzı harita oluşturucu.
/// Her katta birden fazla yol, rastgele node tipleri.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [Header("Map Config")]
    [SerializeField] private int   totalFloors  = 15;
    [SerializeField] private int   pathCount    = 3;
    [SerializeField] private float nodeSpacingX = 120f;
    [SerializeField] private float nodeSpacingY = 80f;

    [Header("Node Weights (0-100)")]
    [SerializeField] private int combatWeight   = 45;
    [SerializeField] private int eliteWeight    = 15;
    [SerializeField] private int restWeight     = 15;
    [SerializeField] private int shopWeight     = 10;
    [SerializeField] private int eventWeight    = 10;
    [SerializeField] private int treasureWeight = 5;

    public List<List<MapNode>> FloorNodes { get; private set; }
    public MapNode CurrentNode { get; private set; }
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void GenerateMap()
    {
        FloorNodes = new List<List<MapNode>>();

        // Her kat için node listesi oluştur
        for (int floor = 0; floor < totalFloors; floor++)
        {
            var floorList = new List<MapNode>();
            int columns   = floor == totalFloors - 1 ? 1 : pathCount; // Boss katı tekil

            for (int col = 0; col < columns; col++)
            {
                var node = new MapNode
                {
                    Floor       = floor,
                    Column      = col,
                    Type        = DetermineNodeType(floor),
                    IsCompleted = false,
                    IsAccessible = floor == 0,
                    UIPosition  = new Vector2(
                        col * nodeSpacingX - (columns - 1) * nodeSpacingX / 2f,
                        floor * nodeSpacingY)
                };
                floorList.Add(node);
            }
            FloorNodes.Add(floorList);
        }

        ConnectNodes();
    }

    private void ConnectNodes()
    {
        for (int floor = 0; floor < FloorNodes.Count - 1; floor++)
        {
            var current = FloorNodes[floor];
            var next    = FloorNodes[floor + 1];

            foreach (var node in current)
            {
                // Her node en az bir sonraki kata bağlanır
                int targetCol = Mathf.Clamp(node.Column, 0, next.Count - 1);
                node.NextNodes.Add(next[targetCol]);

                // Rastgele ek bağlantı
                if (next.Count > 1 && Random.value > 0.5f)
                {
                    int altCol = Random.Range(0, next.Count);
                    if (altCol != targetCol)
                        node.NextNodes.Add(next[altCol]);
                }
            }
        }
    }

    private NodeType DetermineNodeType(int floor)
    {
        // İlk kat her zaman savaş
        if (floor == 0) return NodeType.Combat;

        // Son kat her zaman boss
        if (floor == totalFloors - 1) return NodeType.Boss;

        // 8. kat her zaman dinlenme (zorunlu checkpoint)
        if (floor == 8) return NodeType.Rest;

        return WeightedRandom();
    }

    private NodeType WeightedRandom()
    {
        int total = combatWeight + eliteWeight + restWeight
                  + shopWeight  + eventWeight  + treasureWeight;
        int roll  = Random.Range(0, total);

        if (roll < combatWeight)                      return NodeType.Combat;
        roll -= combatWeight;
        if (roll < eliteWeight)                       return NodeType.Elite;
        roll -= eliteWeight;
        if (roll < restWeight)                        return NodeType.Rest;
        roll -= restWeight;
        if (roll < shopWeight)                        return NodeType.Shop;
        roll -= shopWeight;
        if (roll < eventWeight)                       return NodeType.Event;
        return NodeType.Treasure;
    }

    // --- Görsel & Navigasyon ---

    public void ShowMap()
    {
        UIManager.Instance.ShowMap();
        GetComponent<MapUI>().DrawMap(FloorNodes);
    }


    public void OnNodeSelected(MapNode node)
    {
        if (!node.IsAccessible || node.IsCompleted) return;

        CurrentNode = node;
        Player.Instance.IncrementFloor();
        HandleNodeType(node);
    }

    private void HandleNodeType(MapNode node)
    {
        switch (node.Type)
        {
            case NodeType.Combat:
            case NodeType.Elite:
                var enemies = NodeEventHandler.Instance.GetEnemiesForNode(node);
                RunManager.Instance.SetPendingEncounter(enemies);
                GameStateManager.Instance.TransitionTo(GameState.Combat);
                break;

            case NodeType.Boss:
                var boss = NodeEventHandler.Instance.GetBossEnemies();
                RunManager.Instance.SetPendingEncounter(boss);
                GameStateManager.Instance.TransitionTo(GameState.Combat);
                break;

            case NodeType.Rest:
                GameStateManager.Instance.TransitionTo(GameState.Rest);
                break;

            case NodeType.Shop:
                GameStateManager.Instance.TransitionTo(GameState.Shop);
                break;

            case NodeType.Event:
            case NodeType.Treasure:
                NodeEventHandler.Instance.TriggerEvent(node);
                break;
        }
    }

    public void MarkCurrentNodeComplete()
    {
        if (CurrentNode == null) return;
        CurrentNode.IsCompleted = true;

        foreach (var next in CurrentNode.NextNodes)
            next.IsAccessible = true;

        GetComponent<MapUI>().RefreshMap(FloorNodes);
    }
}