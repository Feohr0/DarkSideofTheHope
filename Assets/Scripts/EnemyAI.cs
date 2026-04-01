using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI
{
    public enum AIStrategy { Aggressive, Defensive, Balanced }

    public AIStrategy strategy;

    public EnemyAI(AIStrategy strategy = AIStrategy.Balanced)
    {
        this.strategy = strategy;
    }

    // Dışarıdan çağrılır — elindeki kartlardan en uygununu seçer
    public Card ChooseCard(Player enemy, Player player)
    {
        List<Card> playable = GetPlayableCards(enemy);

        if (playable.Count == 0) return null;

        switch (strategy)
        {
            case AIStrategy.Aggressive:  return ChooseAggressive(playable);
            case AIStrategy.Defensive:   return ChooseDefensive(enemy, playable);
            case AIStrategy.Balanced:    return ChooseBalanced(enemy, player, playable);
            default:                     return playable[0];
        }
    }

    // Sadece oynanabilir kartları filtrele (enerji yeterli olanlar)
    private List<Card> GetPlayableCards(Player enemy)
    {
        List<Card> playable = new List<Card>();
        foreach (Card card in enemy.hand)
        {
            if (card.energyCost <= enemy.currentEnergy)
                playable.Add(card);
        }
        return playable;
    }

    // Aggressive → en yüksek hasar kartını oyna
    private Card ChooseAggressive(List<Card> playable)
    {
        Card best = null;
        foreach (Card card in playable)
        {
            if (card.effect == Card.EffectType.Damage)
                if (best == null || card.power > best.power)
                    best = card;
        }
        // Hasar kartı yoksa herhangi birini oyna
        return best ?? playable[0];
    }

    // Defensive → önce kalkan, sonra heal, sonra diğerleri
    private Card ChooseDefensive(Player enemy, List<Card> playable)
    {
        // Kalkan yoksa kalkan oyna
        if (enemy.shield == 0)
        {
            Card shieldCard = playable.Find(c => c.effect == Card.EffectType.Shield);
            if (shieldCard != null) return shieldCard;
        }

        // Can azsa heal oyna
        if (enemy.health < 15)
        {
            Card healCard = playable.Find(c => c.effect == Card.EffectType.Heal);
            if (healCard != null) return healCard;
        }

        // Yoksa saldır
        return ChooseAggressive(playable);
    }

    // Balanced → can durumuna ve enerjiye göre karar ver
    private Card ChooseBalanced(Player enemy, Player player, List<Card> playable)
    {
        float hpRatio = (float)enemy.health / 30f;

        // Can %40'ın altına düştüyse defensive davran
        if (hpRatio < 0.4f)
            return ChooseDefensive(enemy, playable);

        // Düşmanın canı çok azsa öldürücü darbe vur
        if (player.health <= 6)
            return ChooseAggressive(playable);

        // Normal durumda: enerjiyi verimli kullan (en yüksek power/cost oranı)
        return ChooseBestValue(playable);
    }

    // En iyi değer/maliyet oranına sahip kartı seç
    private Card ChooseBestValue(List<Card> playable)
    {
        Card best      = null;
        float bestRatio = -1f;

        foreach (Card card in playable)
        {
            float ratio = (float)card.power / card.energyCost;
            if (ratio > bestRatio)
            {
                bestRatio = ratio;
                best      = card;
            }
        }
        return best ?? playable[0];
    }
}