// Scripts/Core/RunManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bir run boyunca kalıcı veriyi tutar: deck, relikler, altın.
/// Sahne değişimlerinde kaybolmaz.
/// </summary>
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public List<CardData>   CurrentDeck   { get; private set; } = new List<CardData>();
    public List<RelicBase>  ActiveRelics  { get; private set; } = new List<RelicBase>();
    public List<EnemyData>  PendingEncounter { get; private set; }

    [Header("Starter Deck")]
    [SerializeField] private List<CardData> starterCards;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun()
    {
        CurrentDeck.Clear();
        ActiveRelics.Clear();
        CurrentDeck.AddRange(starterCards);
        MapGenerator.Instance.GenerateMap();
        GameStateManager.Instance.TransitionTo(GameState.Map);
    }

    public void AddCardToDeck(CardData card)
    {
        CurrentDeck.Add(card);
        Debug.Log("[Run] Deste'ye eklendi: " + card.CardName);
    }

    public void RemoveCardFromDeck(CardData card)
    {
        CurrentDeck.Remove(card);
    }

    public void AddRelic(RelicBase relic)
    {
        ActiveRelics.Add(relic);
        relic.OnObtain();
    }

    public void SetPendingEncounter(List<EnemyData> enemies)
    {
        PendingEncounter = enemies;
    }

    // Relic hook'larını yayınla
    public void NotifyRelicsCombatStart()
    {
        foreach (var r in ActiveRelics) r.OnCombatStart();
    }

    public void NotifyRelicsTurnStart()
    {
        foreach (var r in ActiveRelics) r.OnTurnStart();
    }

    public void NotifyRelicsCardPlayed(CardData card)
    {
        foreach (var r in ActiveRelics) r.OnCardPlayed(card);
    }
}