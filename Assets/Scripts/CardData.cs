using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Card.EffectType effect;
    public Sprite cardArt;

    [Header("Geliştirme Seviyesi (0-3)")]
    public int currentLevel = 0;

    [Header("Seviye Başına Değerler")]
    public int[] powerLevels = new int[4];   // Örn: [5, 7, 10, 15]
    public int[] energyLevels = new int[4];  // Örn: [2, 2, 1, 1]

    // O anki seviyeye göre değerleri döndüren metodlar
    public int GetCurrentPower() => powerLevels[Mathf.Clamp(currentLevel, 0, 3)];
    public int GetCurrentCost() => energyLevels[Mathf.Clamp(currentLevel, 0, 3)];

    public void Upgrade()
    {
        if (currentLevel < 3) currentLevel++;
    }

    // Runtime kart nesnesini oluştururken referansı aktar
    public Card ToCard() => new Card(this);
}