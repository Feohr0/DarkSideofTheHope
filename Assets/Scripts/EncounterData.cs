using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "CardGame/Encounter")]
public class EncounterData : ScriptableObject
{
    [Header("Düşman Bilgileri")]
    public string enemyName;
    public int maxHealth;
    public int maxEnergy = 3;
    
    [Header("Düşman Destesi")]
    public DeckData enemyDeck;
    
    [Header("Görsel")]
    public Sprite enemySprite; // Düşmanın ekrandaki resmi
}