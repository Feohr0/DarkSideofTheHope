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
    
    [Header("Ses")]
    public AudioSource sfxSource;
    public AudioClip attackSfx;
    public AudioClip shieldSfx;
    
    [Header("Desteler (ScriptableObject)")]
    public DeckData playerDeckData;
    public DeckData enemyDeckData;
    
    [Header("3D Çevre")]
    public Transform enemySpawnPoint;       // Sahnedeki EnemySpawnPoint'i buraya sürükle
    private GameObject currentEnemyModel;
    private VisualFXtest activeEnemyFlash;
    private UIShake      activeEnemyShake;

    [Header("Sarsılma Efektleri")]
    [Tooltip("Oyuncu karakter sprite'ına eklenmiş UIShake bileşeni")]
    public UIShake playerCharacterShake;
    
    public bool   IsPlayerTurn      => currentActor == player;
    public string CurrentActorName  => currentActor.playerName;
    private EncounterData currentEncounter;
    
    public GameObject[] enemySprites;
    
    public void InitBattle(DeckData playerDeck, EncounterData encounter, int pMaxHP, int pCurrentHP)
    {
        // Önceki savaşın event'lerini temizle
        if (enemy != null)
        {
            enemy.Damaged -= OnEnemyDamaged;
        }
        if (player != null)
        {
            player.Damaged -= OnPlayerDamaged;
        }

        activeEnemyFlash = null;
        activeEnemyShake = null;
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
            GameObject activeEnemySprite = enemySprites[pickedIndex];
            activeEnemySprite.gameObject.SetActive(true);
            activeEnemyFlash = activeEnemySprite.GetComponent<VisualFXtest>();

            if (activeEnemyFlash == null)
            {
                activeEnemyFlash = activeEnemySprite.AddComponent<VisualFXtest>();
            }

            // Düşman sprite'ına sarsılma bileşeni ekle (yoksa oluştur)
            activeEnemyShake = activeEnemySprite.GetComponent<UIShake>();
            if (activeEnemyShake == null)
            {
                activeEnemyShake = activeEnemySprite.AddComponent<UIShake>();
            }
        }

        // Oyuncuyu mevcut canıyla yarat
        player = new Player("Oyuncu", pMaxHP, pCurrentHP, 6);
        enemy  = new Player(encounter.enemyName, encounter.maxHealth, encounter.maxHealth, encounter.maxEnergy);
        enemy.Damaged  += OnEnemyDamaged;
        player.Damaged += OnPlayerDamaged;

        player.deck = playerDeck.BuildShuffledDeck();
        enemy.deck  = encounter.enemyDeck.BuildShuffledDeck();

        currentActor = player;
        currentTarget = enemy;
        BeginTurn(); 
    }

    // Savaş bitince veya haritaya dönünce modeli temizlemek için
    public void ClearBattlefield()
    {
        if (enemy != null)
        {
            enemy.Damaged -= OnEnemyDamaged;
        }
        if (player != null)
        {
            player.Damaged -= OnPlayerDamaged;
        }

        if (currentEnemyModel != null)
        {
            Destroy(currentEnemyModel);
        }
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.Damaged -= OnEnemyDamaged;
        }
        if (player != null)
        {
            player.Damaged -= OnPlayerDamaged;
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

        // Oyuncunun turu başlıyor → düşman hamle metnini temizle
        if (currentActor == player)
        {
            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null && ui.hudView != null)
                ui.hudView.ClearEnemyAction();
        }

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

            // Son hamle mesajının okunması için bekle
            yield return new WaitForSeconds(1.5f);

            // Enerji bitti → mesaj okundu, turu bitir
            if (enemy.currentEnergy == 0) break;
        }

        // Turu bitirmeden önce son mesajın ekranda kalması için bekle
        if (!gameOver && currentActor == enemy)
        {
            yield return new WaitForSeconds(1.5f);
            EndTurn();
        }
    }

    // Dışarıdan (UI veya AI) çağrılır
    public void TryPlayCard(Card card)
    {
        if (gameOver) return;

        bool played = currentActor.PlayCard(card);
        if (!played) return;
        
        if (currentActor == player)
            PlayCardSfx(card);
        
        if (currentActor == enemy && currentEncounter != null)
        {
            battleSystem.ApplyCard(
                card,
                currentActor,
                currentTarget,
                currentEncounter.damageMultiplier,
                currentEncounter.damageBonus
            );

            // Düşman hamle metnini göster
            ShowEnemyActionMessage(card, currentEncounter.damageMultiplier, currentEncounter.damageBonus);
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
        
            // Düşman öldüyse oyuncu kazandı (true), oyuncu öldüyse kaybetti (false)
            bool playerWon = (currentTarget == enemy);
            FindObjectOfType<GameManager>().EndBattle(playerWon);
            return;
        }

        // Enerji bitti → sadece oyuncunun turuysa burada bitir.
        // Düşman sırasında coroutine hallediyor (bekleme süresi var).
        if (currentActor.currentEnergy == 0 && currentActor == player)
        {
            Debug.Log("Enerji tükendi, tur geçiyor...");
            EndTurn();
        }
    }
    
    private void PlayCardSfx(Card card)
    {
        if (sfxSource == null || card == null) return;
        
        switch (card.effect)
        {
            case Card.EffectType.Damage:
                if (attackSfx != null) sfxSource.PlayOneShot(attackSfx);
                break;
            case Card.EffectType.Shield:
                if (shieldSfx != null) sfxSource.PlayOneShot(shieldSfx);
                break;
        }
    }

    /// <summary>Düşmanın oynadığı kartı Türkçe mesaj olarak HUD'a gönderir.</summary>
    private void ShowEnemyActionMessage(Card card, float damageMultiplier = 1f, int damageBonus = 0)
    {
        if (card == null) return;

        string message = card.effect switch
        {
            Card.EffectType.Damage =>
                $"Düşman {Mathf.RoundToInt(card.power * damageMultiplier) + damageBonus} hasar verdi!",
            Card.EffectType.Shield =>
                $"Düşman {card.power} kalkan edindi!",
            Card.EffectType.Heal =>
                $"Düşman {card.power} can yeniledi!",
            _ => $"Düşman {card.cardName} kartını oynadı."
        };

        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null && ui.hudView != null)
            ui.hudView.ShowEnemyAction(message);
    }


    public void EndTurn()
    {
        if (gameOver) return;
        Debug.Log($"{currentActor.playerName} turu bitirdi.\n");
        (currentActor, currentTarget) = (currentTarget, currentActor);
        BeginTurn();
    }

    private void OnEnemyDamaged(int incomingDamage, int healthLost)
    {
        if (incomingDamage <= 0) return;

        // Düşman sprite flash efekti
        if (healthLost > 0 && activeEnemyFlash != null)
            activeEnemyFlash.TriggerFlash();

        // Düşman sprite sarsılma efekti
        if (healthLost > 0 && activeEnemyShake != null)
            activeEnemyShake.Shake();

        // Düşman can barı sarsılma efekti
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null && ui.hudView != null)
        {
            float normalized = enemy != null && enemy.maxHealth > 0
                ? (float)healthLost / enemy.maxHealth
                : 0.5f;
            ui.hudView.ShakeEnemyHP(normalized);
        }
    }

    private void OnPlayerDamaged(int incomingDamage, int healthLost)
    {
        if (incomingDamage <= 0) return;

        // Oyuncu karakter sprite sarsılma efekti
        if (healthLost > 0 && playerCharacterShake != null)
            playerCharacterShake.Shake();

        // Oyuncu can barı sarsılma efekti
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null && ui.hudView != null)
        {
            float normalized = player != null && player.maxHealth > 0
                ? (float)healthLost / player.maxHealth
                : 0.5f;
            ui.hudView.ShakePlayerHP(normalized);
        }
    }
}
