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
    
    public bool   IsPlayerTurn      => currentActor == player;
    public string CurrentActorName  => currentActor.playerName;
    
    void Start()
    {
        player = new Player("Oyuncu", 30, 3);
        enemy  = new Player("Düşman",  30, 3);

        // ScriptableObject'ten karıştırılmış deste oluştur
        player.deck = playerDeckData.BuildShuffledDeck();
        enemy.deck  = enemyDeckData.BuildShuffledDeck();

        StartGame();
    }

    void StartGame()
    {
        for (int i = 0; i < 3; i++) player.DrawCard();
        for (int i = 0; i < 3; i++) enemy.DrawCard();

        currentActor  = player;
        currentTarget = enemy;
        BeginTurn();
    }

    public void BeginTurn()
    {
        currentActor.shield = 0;
        currentActor.RefillEnergy();
        currentActor.DrawCard();
        Debug.Log($"=== {currentActor.playerName}'ın turu ===");
        Debug.Log(currentActor.ToString());

        // Düşmanın turu başladıysa AI'ı devreye sok
        if (currentActor == enemy)
            StartCoroutine(RunEnemyTurn());
        
        FindObjectOfType<UIManager>()?.Refresh();
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

        battleSystem.ApplyCard(card, currentActor, currentTarget);

        if (!currentTarget.IsAlive)
        {
            Debug.Log($"🏆 {currentActor.playerName} kazandı!");
            gameOver = true;
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