using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public const int MaxUpgradeLevel = 3;

    public string cardName;
    public Card.EffectType effect;
    public Sprite cardArt;

    [Header("Geliştirme Seviyesi (0-3)")]
    public int currentLevel = 0;

    [Header("Seviye Başına Değerler")]
    public int[] powerLevels = new int[4];   // Örn: [5, 7, 10, 15]
    public int[] energyLevels = new int[4];  // Örn: [2, 2, 1, 1]

    // O anki seviyeye göre değerleri döndüren metodlar
    public int GetCurrentPower() => powerLevels[GetClampedLevel(currentLevel)];
    public int GetCurrentCost() => energyLevels[GetClampedLevel(currentLevel)];

    public bool CanUpgrade() => currentLevel < MaxUpgradeLevel;

    public int GetPowerAtLevel(int level) => powerLevels[GetClampedLevel(level)];
    public int GetCostAtLevel(int level) => energyLevels[GetClampedLevel(level)];

    public void Upgrade()
    {
        if (CanUpgrade()) currentLevel++;
    }

    // Runtime kart nesnesini oluştururken referansı aktar
    public Card ToCard() => new Card(this);

    private int GetClampedLevel(int level) => Mathf.Clamp(level, 0, MaxUpgradeLevel);
}
