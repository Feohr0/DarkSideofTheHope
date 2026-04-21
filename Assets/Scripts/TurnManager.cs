using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public Player enemy;

    private Player currentActor;
    private Player currentTarget;          // aktörün rakibi
    private BattleSystem battleSystem = new BattleSystem();
    private EnemyAI      enemyAI      = new EnemyAI(EnemyAI.AIStrategy.Balanced);
    
    private bool gameOver = false;
    
    [Header("Desteler (ScriptableObject)")]
    public DeckData playerDeckData;
    public DeckData enemyDeckData;
    
    [Header("3D Çevre")]
    public Transform enemySpawnPoint;       // Sahnedeki EnemySpawnPoint'i buraya sürükle
    private GameObject currentEnemyModel;
    
    public bool   IsPlayerTurn      => currentActor == player;
    public string CurrentActorName  => currentActor.playerName;
    private EncounterData currentEncounter;
    
    public GameObject[] enemySprites;
    
    public void InitBattle(DeckData playerDeck, EncounterData encounter, int pMaxHP, int pCurrentHP)
    {
        currentEncounter = encounter;
        gameOver = false;
        StopAllCoroutines();
        ClearBattlefield();

        if (encounter.enemyPrefab != null && enemySpawnPoint != null)
        {
         //   currentEnemyModel = Instantiate(encounter.enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
        }

        foreach (GameObject e in enemySprites )
        {
            e.SetActive(false);
        }

        int pickedIndex = encounter != null ? encounter.PickEnemyIndex() : 0;
        if (enemySprites != null && enemySprites.Length > 0)
        {
            pickedIndex = Mathf.Clamp(pickedIndex, 0, enemySprites.Length - 1);
            enemySprites[pickedIndex].gameObject.SetActive(true);
        }

        // Oyuncuyu mevcut canıyla yarat
        player = new Player("Oyuncu", pMaxHP, pCurrentHP, 6);
        enemy  = new Player(encounter.enemyName, encounter.maxHealth, encounter.maxHealth, encounter.maxEnergy);

        player.deck = playerDeck.BuildShuffledDeck();
        enemy.deck  = encounter.enemyDeck.BuildShuffledDeck();

        currentActor = player;
        currentTarget = enemy;
        BeginTurn(); 
    }

    // Savaş bitince veya haritaya dönünce modeli temizlemek için
    public void ClearBattlefield()
    {
        if (currentEnemyModel != null)
        {
            Destroy(currentEnemyModel);
        }
    }

    public void BeginTurn()
    {
        currentActor.shield = 0;
        currentActor.RefillEnergy();

        // Tur başında eli temizle (1. seçeneğin gereği)
        currentActor.hand.Clear();

        // 4 kart çekmeye çalış
        for (int i = 0; i < 4; i++) 
        {
            bool hasCard = currentActor.DrawCard();

            // Eğer kart çekilemediyse ve sıra oyuncudaysa oyunu bitir
            if (!hasCard && currentActor == player)
            {
                Debug.Log("Deste tükendi! Kaynakların bittiği için yenildin.");
                FindObjectOfType<GameManager>().EndBattle(false); // Kaybetme durumu
                return; // Fonksiyondan çık ki hata vermesin
            }
        }
    
        FindObjectOfType<UIManager>()?.Refresh();

        if (currentActor == enemy)
            StartCoroutine(RunEnemyTurn());
    }
    
    private IEnumerator RunEnemyTurn()
    {
        Debug.Log("Düşman düşünüyor...");
        yield return new WaitForSeconds(1f);

        while (true)
        {
            Card chosen = enemyAI.ChooseCard(enemy, player);

            // Oynayacak kart kalmadı veya enerji bitti
            if (chosen == null)
            {
                Debug.Log("Düşman oynayacak kart bulamadı.");
                break;
            }

            TryPlayCard(chosen);

            if (gameOver) yield break;

            // Enerji bitti → döngüden çık (EndTurn zaten çağrıldı)
            if (enemy.currentEnergy == 0) yield break;

            yield return new WaitForSeconds(0.8f);   // kartlar arası bekleme
        }

        // Hâlâ enerji varsa turu manuel bitir
        if (!gameOver && currentActor == enemy)
            EndTurn();
    }

    // Dışarıdan (UI veya AI) çağrılır
    public void TryPlayCard(Card card)
    {
        if (gameOver) return;

        bool played = currentActor.PlayCard(card);
        if (!played) return;
        
        if (currentActor == enemy && currentEncounter != null)
        {
            battleSystem.ApplyCard(
                card,
                currentActor,
                currentTarget,
                currentEncounter.damageMultiplier,
                currentEncounter.damageBonus
            );
        }
        else
        {
            battleSystem.ApplyCard(card, currentActor, currentTarget);
        }

        if (!currentTarget.IsAlive)
        {
            gameOver = true;
            // Düşman öldüğünde parayı gönder
            if (currentTarget == enemy) 
            {
                FindObjectOfType<GameManager>().AddGold(currentEncounter.goldReward);
            }
        
            FindObjectOfType<GameManager>().EndBattle(true);
            return;
        }

        if (currentActor.currentEnergy == 0)
        {
            Debug.Log("Enerji tükendi, tur geçiyor...");
            EndTurn();
        }
    }

   
    public void EndTurn()
    {
        if (gameOver) return;
        Debug.Log($"{currentActor.playerName} turu bitirdi.\n");
        (currentActor, currentTarget) = (currentTarget, currentActor);
        BeginTurn();
    }
}