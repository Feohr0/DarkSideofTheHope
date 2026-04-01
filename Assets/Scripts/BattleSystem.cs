using UnityEngine;

public class BattleSystem
{
    // Bir oyuncunun kartını rakibe (veya kendine) uygulaması
    public void ApplyCard(Card card, Player caster, Player target)
    {
        Debug.Log($"{caster.playerName} → {card} kullanıyor...");

        switch (card.effect)
        {
            case Card.EffectType.Damage:
                target.TakeDamage(card.power);
                break;

            case Card.EffectType.Shield:
                caster.AddShield(card.power);   // kalkan oynayan kişiye gider
                break;

            case Card.EffectType.Heal:
                caster.HealHealth(card.power);  // heal de oynayan kişiye gider
                break;
        }

        CheckDeathState(target);
    }

    private void CheckDeathState(Player target)
    {
        if (!target.IsAlive)
            Debug.Log($"💀 {target.playerName} yenildi!");
    }
}