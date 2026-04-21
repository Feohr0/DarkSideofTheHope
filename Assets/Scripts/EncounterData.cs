using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "CardGame/Encounter")]
public class EncounterData : ScriptableObject
{
    [Header("Düşman Bilgileri")]
    public string enemyName = "DÜŞMAN";
    public int maxHealth;
    public int maxEnergy = 3;

    public int enemyIndex;
    
    [Header("Ödüller")]
    public int goldReward; // Bu düşmanı yenince kaç coin gelecek?
    
    [Header("Düşman Destesi")]
    public DeckData enemyDeck;
    
    [Header("Görsel")]
    //public Sprite enemySprite; // Düşmanın ekrandaki resmi
    public GameObject enemyPrefab;
}