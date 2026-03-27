// Scripts/Combat/CombatManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private GameObject  enemyPrefab;

    public List<Enemy> ActiveEnemies { get; } = new List<Enemy>();
    public bool IsPlayerTurn { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // -------------------------------------------------------
    // Combat Lifecycle
    // -------------------------------------------------------

    public void StartCombat()
    {
        ActiveEnemies.Clear();
        SpawnEnemies(RunManager.Instance.PendingEncounter);
        Player.Instance.GetComponent<DeckManager>()
              .InitializeDeck(RunManager.Instance.CurrentDeck);
        RunManager.Instance.NotifyRelicsCombatStart();
        BeginPlayerTurn();
    }

    public void CleanupCombat()
    {
        foreach (var e in ActiveEnemies)
            if (e != null) Destroy(e.gameObject);
        ActiveEnemies.Clear();
        Player.Instance.GetComponent<DeckManager>().DiscardHand();
    }

    public void OnEnemyDied(Enemy enemy)
    {
        ActiveEnemies.Remove(enemy);
        if (ActiveEnemies.Count == 0)
            StartCoroutine(WinCombatRoutine());
    }

    public void OnEndTurnButtonPressed()
    {
        if (!IsPlayerTurn) return;
        StartCoroutine(EndPlayerTurnRoutine());
    }

    // -------------------------------------------------------
    // Turn Logic (TurnManager buraya taşındı)
    // -------------------------------------------------------

    private void BeginPlayerTurn()
    {
        IsPlayerTurn = true;
        Player.Instance.StartCombatTurn();
        RunManager.Instance.NotifyRelicsTurnStart();
        EventBus.Publish(new TurnStartedEvent(isPlayerTurn: true));
    }

    private IEnumerator EndPlayerTurnRoutine()
    {
        IsPlayerTurn = false;
        Player.Instance.GetComponent<DeckManager>().DiscardHand();
        EventBus.Publish(new TurnStartedEvent(isPlayerTurn: false));

        foreach (var enemy in ActiveEnemies)
        {
            if (!enemy.IsAlive) continue;
            yield return new WaitForSeconds(0.6f);
            enemy.ExecuteIntent(Player.Instance);
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.3f);

        if (ActiveEnemies.Count > 0)
            BeginPlayerTurn();
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private void SpawnEnemies(List<EnemyData> data)
    {
        if (data == null || data.Count == 0) return;

        for (int i = 0; i < Mathf.Min(data.Count, enemySpawnPoints.Length); i++)
        {
            var go    = Instantiate(enemyPrefab,
                                    enemySpawnPoints[i].position,
                                    enemySpawnPoints[i].rotation);
            var enemy = go.GetComponent<Enemy>();
            enemy.Initialize(data[i]);
            ActiveEnemies.Add(enemy);
        }
    }

    private IEnumerator WinCombatRoutine()
    {
        yield return new WaitForSeconds(1.2f);
        MapGenerator.Instance.MarkCurrentNodeComplete();

        var rewardCards = GenerateRewardCards();
        UIManager.Instance.ShowReward(rewardCards);
    }

    private List<CardData> GenerateRewardCards()
    {
        // CardDatabase'den 3 rastgele kart seç
        // Şimdilik boş liste, CardDatabase eklenince doldurulur
        return new List<CardData>();
    }
}