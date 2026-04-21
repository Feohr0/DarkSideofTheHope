using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "CardGame/Encounter")]
public class EncounterData : ScriptableObject
{
    [Header("Düşman Bilgileri")]
    public string enemyName = "DÜŞMAN";
    public int maxHealth;
    public int maxEnergy = 3;

    public int enemyIndex;
    
    [Header("Düşman Hasarı (Kart Hasarına Uygulanır)")]
    [Tooltip("Düşmanın DAMAGE kartlarından gelen hasara eklenecek sabit değer.")]
    public int damageBonus = 0;
    
    [Tooltip("Düşmanın DAMAGE kartlarından gelen hasara uygulanacak çarpan.")]
    public float damageMultiplier = 1f;
    
    [Header("Ödüller")]
    public int goldReward; // Bu düşmanı yenince kaç coin gelecek?
    
    [Header("Düşman Destesi")]
    public DeckData enemyDeck;
    
    [Header("Görsel")]
    //public Sprite enemySprite; // Düşmanın ekrandaki resmi
    public GameObject enemyPrefab;
}