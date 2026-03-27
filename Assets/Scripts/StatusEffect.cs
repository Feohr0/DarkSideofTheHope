// Scripts/Combat/StatusEffects/StatusEffect.cs
using UnityEngine;

/// <summary>
/// Tüm durum efektlerinin soyut tabanı.
/// Her efekt kendi davranışını override eder.
/// </summary>
public abstract class StatusEffect
{
    public int Stacks { get; protected set; }
    public abstract string Name { get; }
    public abstract string IconKey { get; }

    protected StatusEffect(int stacks) => Stacks = stacks;

    public virtual void Stack(int amount) => Stacks += amount;
    public virtual void OnApply(Entity target) { }
    public abstract void OnTurnStart(Entity owner);
}

// --- Poison ---
public class PoisonEffect : StatusEffect
{
    public override string Name    => "Zehir";
    public override string IconKey => "poison";

    public PoisonEffect(int stacks) : base(stacks) { }

    public override void OnTurnStart(Entity owner)
    {
        owner.TakeDamage(Stacks);
        Stacks--;
    }
}

// --- Burn ---
public class BurnEffect : StatusEffect
{
    public override string Name    => "Yanma";
    public override string IconKey => "burn";

    public BurnEffect(int stacks) : base(stacks) { }

    public override void OnTurnStart(Entity owner)
    {
        owner.TakeDamage(Stacks * 2);
        Stacks = 0; // Yanma bir turda tüketilir
    }
}

// --- Strength ---
public class StrengthEffect : StatusEffect
{
    public override string Name    => "Güç";
    public override string IconKey => "strength";

    public StrengthEffect(int stacks) : base(stacks) { }

    public override void OnTurnStart(Entity owner) { }
    // Güç kalıcıdır, tur başında bir şey yapmaz
}

// --- Vulnerable ---
public class VulnerableEffect : StatusEffect
{
    public override string Name    => "Savunmasız";
    public override string IconKey => "vulnerable";

    public VulnerableEffect(int stacks) : base(stacks) { }

    public override void OnTurnStart(Entity owner) => Stacks--;
    // Hasar hesabında CombatManager %50 bonus uygular
}

// --- Weak ---
public class WeakEffect : StatusEffect
{
    public override string Name    => "Zayıf";
    public override string IconKey => "weak";

    public WeakEffect(int stacks) : base(stacks) { }

    public override void OnTurnStart(Entity owner) => Stacks--;
}