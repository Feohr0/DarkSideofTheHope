// Scripts/Cards/CardData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "StS/Card", fileName = "New Card")]
public class CardData : ScriptableObject
{
    [Header("Identity")]
    public string CardName;
    public string Description;
    public Sprite Artwork;
    public CardType Type;
    public CardRarity Rarity;

    [Header("Cost")]
    public int EnergyCost;

    [Header("Effects")]
    public CardEffect[] Effects; // Composable effect dizisi

    public string GetFormattedDescription()
    {
        // Description içindeki {value} placeholder'larını doldurur
        string desc = Description;
        foreach (var effect in Effects)
            desc = desc.Replace("{value}", effect.Value.ToString());
        return desc;
    }
}

public enum CardType   { Attack, Skill, Power, Status, Curse }
public enum CardRarity { Common, Uncommon, Rare }

[System.Serializable]
public class CardEffect
{
    public CardEffectType EffectType;
    public int Value;
    public TargetType Target;
}

public enum CardEffectType
{
    DealDamage, GainBlock, ApplyPoison, ApplyBurn,
    ApplyStrength, ApplyVulnerable, DrawCards, GainEnergy,
    DoubleAttack, AOEDamage
}

public enum TargetType { Enemy, AllEnemies, Self, Random }
public enum IntentType { Attack, Block, Buff, Debuff, Unknown }