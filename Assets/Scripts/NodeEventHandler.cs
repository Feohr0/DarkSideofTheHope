// Scripts/Map/NodeEventHandler.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Harita nodelarına karşılık gelen düşman ve olay içeriklerini yönetir.
/// </summary>
public class NodeEventHandler : MonoBehaviour
{
    public static NodeEventHandler Instance { get; private set; }

    [Header("Enemy Pools")]
    [SerializeField] private List<EnemyData> commonEnemies;
    [SerializeField] private List<EnemyData> eliteEnemies;
    [SerializeField] private List<EnemyData> bossEnemies;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public List<EnemyData> GetEnemiesForNode(MapNode node)
    {
        var pool   = node.Type == NodeType.Elite ? eliteEnemies : commonEnemies;
        var result = new List<EnemyData>();
        int count  = node.Type == NodeType.Elite ? 1 : Random.Range(1, 3);

        for (int i = 0; i < count; i++)
            result.Add(pool[Random.Range(0, pool.Count)]);

        return result;
    }

    public List<EnemyData> GetBossEnemies()
    {
        return new List<EnemyData> { bossEnemies[Random.Range(0, bossEnemies.Count)] };
    }

    public void TriggerEvent(MapNode node)
    {
        switch (node.Type)
        {
            case NodeType.Treasure:
                int gold = Random.Range(25, 50);
                Player.Instance.GainGold(gold);
                Debug.Log("[Event] Hazine: +" + gold + " Altın");
                GameStateManager.Instance.TransitionTo(GameState.Map);
                break;

            case NodeType.Event:
                // Genişletmek için: RandomEventDatabase'den bir event çek
                Debug.Log("[Event] Rastgele olay tetiklendi.");
                GameStateManager.Instance.TransitionTo(GameState.Map);
                break;
        }
    }
}