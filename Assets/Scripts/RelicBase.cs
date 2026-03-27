// Scripts/Relics/RelicBase.cs
using UnityEngine;

/// <summary>
/// Tüm reliklerin soyut tabanı.
/// Her relic gerektiği event hook'unu override eder.
/// </summary>
public abstract class RelicBase : ScriptableObject
{
    public string RelicName;
    public string Description;
    public Sprite Icon;
    public RelicRarity Rarity;

    public virtual void OnObtain()   { }
    public virtual void OnCombatStart() { }
    public virtual void OnTurnStart()   { }
    public virtual void OnCardPlayed(CardData card) { }
    public virtual void OnDamageDealt(int amount)   { }
    public virtual void OnCombatEnd()   { }
}

public enum RelicRarity { Common, Uncommon, Rare, Boss, Starter }

// Örnek Relic: Her savaşın başında 1 enerji fazladan
[CreateAssetMenu(menuName = "StS/Relics/CoffeeDreg")]
public class CoffeeDregRelic : RelicBase
{
    public override void OnCombatStart()
    {
        // Enerji sistemi üzerinden +1 max enerji
        Debug.Log($"[Relic] {RelicName}: Bu savaş +1 enerji!");
    }
}

// Örnek Relic: İlk kart oynandığında +1 daha çek
[CreateAssetMenu(menuName = "StS/Relics/Bag")]
public class BagRelic : RelicBase
{
    private bool _triggered;

    public override void OnCombatStart() => _triggered = false;

    public override void OnCardPlayed(CardData card)
    {
        if (_triggered) return;
        _triggered = true;
        Player.Instance.GetComponent<DeckManager>().DrawCards(1);
    }
}