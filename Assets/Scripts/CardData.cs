using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    [Header("Temel Bilgiler")]
    public string     cardName;
    public int        energyCost;
    public int        power;
    public Card.EffectType effect;

    [Header("Görsel")]
    public Sprite     cardArt;
    public string     flavorText;   // kart altı lore yazısı

    // Runtime'da kullanılan Card nesnesine dönüştür
    public Card ToCard() => new Card(cardName, energyCost, power,
        effect, cardArt, flavorText);
}