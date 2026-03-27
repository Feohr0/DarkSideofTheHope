// Scripts/Entities/Entity.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tüm savaşan varlıkların soyut tabanı. Player ve Enemy bu sınıfı genişletir.
/// IDamageable ve ICardTarget interface'lerini uygular.
/// </summary>
public abstract class Entity : MonoBehaviour, IDamageable, ICardTarget
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int maxBlock  = 999;

    public int CurrentHealth { get; protected set; }
    public int CurrentBlock  { get; protected set; }
    public bool IsAlive      => CurrentHealth > 0;
    public string EntityName => entityName;
    
    [SerializeField] private string entityName = "Entity";

    protected readonly List<StatusEffect> activeEffects = new();
    
    public int GetMaxHealth() => maxHealth;
    
    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentBlock  = 0;
    }

    // --- IDamageable ---
    public virtual void TakeDamage(int amount)
    {
        int blocked = Mathf.Min(CurrentBlock, amount);
        CurrentBlock  -= blocked;
        int remaining  = amount - blocked;
        CurrentHealth  = Mathf.Max(0, CurrentHealth - remaining);

        EventBus.Publish(new DamageDealtEvent(null, this, remaining));

        if (!IsAlive)
            OnDeath();
    }

    public virtual void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }

    public virtual void AddBlock(int amount)
    {
        CurrentBlock = Mathf.Min(maxBlock, CurrentBlock + amount);
    }

    // --- Status Effects ---
    public void ApplyEffect(StatusEffect effect)
    {
        var existing = activeEffects.Find(e => e.GetType() == effect.GetType());
        if (existing != null)
            existing.Stack(effect.Stacks);
        else
        {
            activeEffects.Add(effect);
            effect.OnApply(this);
        }
    }

    public void ProcessEffectsOnTurnStart()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].OnTurnStart(this);
            if (activeEffects[i].Stacks <= 0)
                activeEffects.RemoveAt(i);
        }
    }

    public bool HasEffect<T>() where T : StatusEffect
        => activeEffects.Exists(e => e is T);

    public int GetEffectStacks<T>() where T : StatusEffect
    {
        var e = activeEffects.Find(x => x is T);
        return e?.Stacks ?? 0;
    }

    protected virtual void OnDeath()
    {
        EventBus.Publish(new EntityDiedEvent(this));
    }
}

// --- Interfaces ---
public interface IDamageable
{
    void TakeDamage(int amount);
    void Heal(int amount);
    void AddBlock(int amount);
}

public interface ICardTarget
{
    string EntityName { get; }
    bool IsAlive { get; }
}