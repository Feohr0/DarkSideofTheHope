using UnityEngine;

[System.Serializable]
public class Card
{
    public enum EffectType { Damage, Shield, Heal }

    // Kartın temel verilerini tutan referans (ScriptableObject)
    public CardData data;

    // Kurucu Metot (Constructor): Runtime'da desteye kart eklenirken çalışır
    public Card(CardData data)
    {
        this.data = data;
    }

    // --- Dinamik Özellikler (Veriyi her an CardData'dan çeker) ---

    // İsim: Eğer kartın türü geliştirilmişse (seviyesi > 0), ismin yanına +1, +2 yazar
    public string cardName => data.cardName + (data.currentLevel > 0 ? $" +{data.currentLevel}" : "");

    // Maliyet ve Güç: O anki seviyeye göre doğrudan CardData'dan hesaplanıp gelir
    public int energyCost => data.GetCurrentCost();
    public int power      => data.GetCurrentPower();

    // Diğer görsel ve kural bazlı özellikler
    public EffectType effect => data.effect;
    public Sprite art        => data.cardArt;
    //public string flavorText => data.flavorText;

    // Konsol logları için okunabilir format
    public override string ToString() => $"[{cardName}] Cost:{energyCost} {effect}:{power}";
}