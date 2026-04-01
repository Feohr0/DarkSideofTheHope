using System.Collections;
using UnityEngine;

[System.Serializable]
public class Card
{
    public enum EffectType { Damage, Shield, Heal }

    public string     cardName;
    public int        energyCost;
    public int        power;
    public EffectType effect;
    public Sprite     art;          // CardData'dan taşınan görsel
    public string     flavorText;

    public Card(string name, int cost, int power,
        EffectType effect, Sprite art = null, string flavor = "")
    {
        this.cardName   = name;
        this.energyCost = cost;
        this.power      = power;
        this.effect     = effect;
        this.art        = art;
        this.flavorText = flavor;
    }

    public override string ToString()
        => $"[{cardName}] Cost:{energyCost} {effect}:{power}";
}