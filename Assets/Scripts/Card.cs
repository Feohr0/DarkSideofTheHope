[System.Serializable]
public class Card
{
    public enum EffectType { Damage, Shield, Heal }

    public string     cardName;
    public int        energyCost;
    public int        power;        // attack yerine genel "güç" değeri
    public EffectType effect;

    public Card(string name, int cost, int power, EffectType effect)
    {
        this.cardName   = name;
        this.energyCost = cost;
        this.power      = power;
        this.effect     = effect;
    }

    public override string ToString()
        => $"[{cardName}] Cost:{energyCost} {effect}:{power}";
}