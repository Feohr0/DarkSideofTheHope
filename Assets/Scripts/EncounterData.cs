using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "CardGame/Encounter")]
public class EncounterData : ScriptableObject
{
    [Header("Düşman Bilgileri")]
    public string enemyName = "DÜŞMAN";
    public int maxHealth;
    public int maxEnergy = 3;

    [Tooltip("Tek bir düşman görseli kullanıyorsan burayı doldur.")]
    public int enemyIndex;
    
    [Tooltip("Birden fazla değer girersen, encounter başında random seçilir.")]
    public List<int> enemyIndexPool = new List<int>();
    
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

    public int PickEnemyIndex()
    {
        if (enemyIndexPool != null && enemyIndexPool.Count > 0)
        {
            if (enemyIndexPool.Count == 1) return enemyIndexPool[0];
            return enemyIndexPool[Random.Range(0, enemyIndexPool.Count)];
        }

        return enemyIndex;
    }
}