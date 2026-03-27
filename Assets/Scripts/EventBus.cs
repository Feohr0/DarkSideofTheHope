// Scripts/Utils/EventBus.cs
using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.ContainsKey(type))
            _handlers[type] = Delegate.Combine(_handlers[type], handler);
        else
            _handlers[type] = handler;
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.ContainsKey(type))
            _handlers[type] = Delegate.Remove(_handlers[type], handler);
    }

    public static void Publish<T>(T evt)
    {
        if (_handlers.TryGetValue(typeof(T), out var del))
            (del as Action<T>)?.Invoke(evt);
    }
}

// -----------------------------------------------
// Event tipleri — record yerine class kullan
// -----------------------------------------------

public class CardPlayedEvent
{
    public CardData Card     { get; }
    public Entity   Caster   { get; }
    public Entity   Target   { get; }

    public CardPlayedEvent(CardData card, Entity caster, Entity target)
    {
        Card   = card;
        Caster = caster;
        Target = target;
    }
}

public class DamageDealtEvent
{
    public Entity Source { get; }
    public Entity Target { get; }
    public int    Amount { get; }

    public DamageDealtEvent(Entity source, Entity target, int amount)
    {
        Source = source;
        Target = target;
        Amount = amount;
    }
}

public class EntityDiedEvent
{
    public Entity Entity { get; }

    public EntityDiedEvent(Entity entity)
    {
        Entity = entity;
    }
}

public class TurnStartedEvent
{
    public bool IsPlayerTurn { get; }

    public TurnStartedEvent(bool isPlayerTurn)
    {
        IsPlayerTurn = isPlayerTurn;
    }
}

public class EnergyChangedEvent
{
    public int Current { get; }
    public int Max     { get; }

    public EnergyChangedEvent(int current, int max)
    {
        Current = current;
        Max     = max;
    }
}

public class CardDrawnEvent
{
    public CardData Card { get; }

    public CardDrawnEvent(CardData card)
    {
        Card = card;
    }
}