// Scripts/Cards/CardEffectResolver.cs
using UnityEngine;

/// <summary>
/// Kart efektlerini yorumlar ve uygular.
/// Yeni bir efekt tipi eklemek için sadece switch'e yeni case yeterli.
/// </summary>
public class CardEffectResolver : MonoBehaviour
{
    public static CardEffectResolver Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Resolve(CardData card, Entity caster, Entity primaryTarget)
    {
        foreach (var effect in card.Effects)
        {
            var targets = ResolveTargets(effect.Target, caster, primaryTarget);

            foreach (var target in targets)
                ApplySingleEffect(effect, caster, target);
        }

        EventBus.Publish(new CardPlayedEvent(card, caster, primaryTarget));
    }

    private void ApplySingleEffect(CardEffect effect, Entity caster, Entity target)
    {
        int value = CalculateValue(effect, caster);

        switch (effect.EffectType)
        {
            case CardEffectType.DealDamage:
                target.TakeDamage(value);
                SpawnHitVFX(target.transform.position);
                break;

            case CardEffectType.GainBlock:
                caster.AddBlock(value);
                break;

            case CardEffectType.ApplyPoison:
                target.ApplyEffect(new PoisonEffect(value));
                break;

            case CardEffectType.ApplyBurn:
                target.ApplyEffect(new BurnEffect(value));
                break;

            case CardEffectType.ApplyStrength:
                caster.ApplyEffect(new StrengthEffect(value));
                break;

            case CardEffectType.ApplyVulnerable:
                target.ApplyEffect(new VulnerableEffect(value));
                break;

            case CardEffectType.DrawCards:
                Player.Instance.GetComponent<DeckManager>().DrawCards(value);
                break;

            case CardEffectType.GainEnergy:
                Player.Instance.TrySpendEnergy(-value); // negatif = kazanım
                break;

            case CardEffectType.DoubleAttack:
                target.TakeDamage(value);
                target.TakeDamage(value);
                break;

            case CardEffectType.AOEDamage:
                // Tüm düşmanlara (zaten targets listesinde çözüldü)
                target.TakeDamage(value);
                break;
        }
    }

    private int CalculateValue(CardEffect effect, Entity caster)
    {
        int val = effect.Value;

        // Güç bonusu saldırılara eklenir
        if (effect.EffectType == CardEffectType.DealDamage ||
            effect.EffectType == CardEffectType.DoubleAttack)
        {
            val += caster.GetEffectStacks<StrengthEffect>();

            // Vulnerable debuff: %50 ekstra hasar
            // (target'ı burada almak için farklı bir yaklaşım gerekir,
            //  basitlik için CombatManager'dan sorgulayabiliriz)
        }

        return Mathf.Max(0, val);
    }

    private System.Collections.Generic.List<Entity> ResolveTargets(
        TargetType targetType, Entity caster, Entity primary)
    {
        var list = new System.Collections.Generic.List<Entity>();
        switch (targetType)
        {
            case TargetType.Enemy:
                if (primary != null) list.Add(primary);
                break;
            case TargetType.AllEnemies:
                list.AddRange(CombatManager.Instance.ActiveEnemies);
                break;
            case TargetType.Self:
                list.Add(caster);
                break;
            case TargetType.Random:
                var enemies = CombatManager.Instance.ActiveEnemies;
                if (enemies.Count > 0)
                    list.Add(enemies[Random.Range(0, enemies.Count)]);
                break;
        }
        return list;
    }

    private void SpawnHitVFX(Vector3 position)
    {
        // VFX pool'dan al ve oynat
        // VFXManager.Instance.PlayHit(position);
    }
}