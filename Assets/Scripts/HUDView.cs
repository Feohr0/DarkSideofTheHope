using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// Üst/alt HUD: can barı, enerji, tur yazısı
public class HUDView : MonoBehaviour
{
    [Header("Oyuncu")]
    public Slider            playerHPBar;
    public TextMeshProUGUI   playerHPText;
    public TextMeshProUGUI   playerEnergyText;
    public TextMeshProUGUI   playerShieldText;

    [Header("Oyuncu Sarsılma")]
    [Tooltip("Oyuncunun can barına (veya üst grubuna) eklenmiş UIShake bileşeni")]
    public UIShake           playerHPShake;

    [Header("Düşman")]
    public Slider            enemyHPBar;
    public TextMeshProUGUI   enemyHPText;
    public TextMeshProUGUI   enemyShieldText;

    [Header("Düşman Sarsılma")]
    [Tooltip("Düşmanın can barına (veya üst grubuna) eklenmiş UIShake bileşeni")]
    public UIShake           enemyHPShake;
    [Tooltip("Düşmanın son hamlesini gösteren text (\"Düşman 5 hasar verdi\" vb.)")]
    public TextMeshProUGUI   enemyActionText;

    [Header("Genel")]
    public TextMeshProUGUI   turnText;
    public Button            endTurnButton;
    public TextMeshProUGUI   logText;          // Son olay mesajı

    private int playerMaxHP;
    private int enemyMaxHP;

    [Header("Ekonomi UI")]
    public TextMeshProUGUI goldText;

    [Header("Can Barı Geçiş Ayarları")]
    [Tooltip("Can barının hedef değere ulaşma süresi (saniye)")]
    [SerializeField] private float hpBarSmoothTime = 0.4f;

    // Coroutine takibi — aynı anda birden fazla geçiş olmasın
    private Coroutine playerHPLerpCoroutine;
    private Coroutine enemyHPLerpCoroutine;

    private void Awake()
    {
        // Inspector'dan atanmadıysa, can barı slider'larına otomatik UIShake ekle
        if (playerHPShake == null && playerHPBar != null)
        {
            playerHPShake = playerHPBar.GetComponent<UIShake>();
            if (playerHPShake == null)
                playerHPShake = playerHPBar.gameObject.AddComponent<UIShake>();
        }

        if (enemyHPShake == null && enemyHPBar != null)
        {
            enemyHPShake = enemyHPBar.GetComponent<UIShake>();
            if (enemyHPShake == null)
                enemyHPShake = enemyHPBar.gameObject.AddComponent<UIShake>();
        }
    }

    // UIManager.cs içindeki Refresh fonksiyonunda bunu çağırabilirsin
    public void UpdateGold(int currentGold)
    {
        if (goldText != null) goldText.text = currentGold.ToString();
    }
    
    public void Init(int pMaxHP, int eMaxHP)
    {
        playerMaxHP = pMaxHP;
        enemyMaxHP  = eMaxHP;
        
        Debug.Log(playerMaxHP + eMaxHP);
    }

    public void UpdatePlayer(Player p)
    {
        float targetValue = (float)p.health / p.maxHealth;
        playerHPText.text     = $"{p.health} / {p.maxHealth}";
        playerEnergyText.text = $"mana: {p.currentEnergy} / {p.maxEnergy}";
        playerShieldText.text = p.shield > 0 ? $"kalkan: {p.shield}" : "";

        // Yumuşak geçiş
        if (playerHPLerpCoroutine != null)
            StopCoroutine(playerHPLerpCoroutine);
        playerHPLerpCoroutine = StartCoroutine(LerpSlider(playerHPBar, targetValue, () => playerHPLerpCoroutine = null));
    }

    public void UpdateEnemy(Player e)
    {
        float targetValue = (float)e.health / e.maxHealth;
        enemyHPText.text     = $"{e.health} / {e.maxHealth}";
        enemyShieldText.text = e.shield > 0 ? $"kalkan: {e.shield}" : "";

        // Yumuşak geçiş
        if (enemyHPLerpCoroutine != null)
            StopCoroutine(enemyHPLerpCoroutine);
        enemyHPLerpCoroutine = StartCoroutine(LerpSlider(enemyHPBar, targetValue, () => enemyHPLerpCoroutine = null));
    }

    /// <summary>
    /// Slider değerini mevcut konumdan hedef değere yumuşakça geçirir.
    /// </summary>
    private IEnumerator LerpSlider(Slider slider, float target, System.Action onComplete)
    {
        float velocity = 0f;

        while (Mathf.Abs(slider.value - target) > 0.001f)
        {
            slider.value = Mathf.SmoothDamp(slider.value, target, ref velocity, hpBarSmoothTime);
            yield return null;
        }

        slider.value = target;
        onComplete?.Invoke();
    }

    public void SetTurnText(string actorName)
        => turnText.text = $"Sıra {actorName}da";

    public void SetEndTurnInteractable(bool state)
        => endTurnButton.interactable = state;

    public void ShowLog(string message)
        => logText.text = message;

    /// <summary>Düşmanın oynadığı kartın açıklamasını ekrana yaz.</summary>
    public void ShowEnemyAction(string message)
    {
        if (enemyActionText != null)
            enemyActionText.text = message;
    }

    /// <summary>Düşman hamle text'ini temizle (oyuncu turu başlıyınca vb.).</summary>
    public void ClearEnemyAction()
    {
        if (enemyActionText != null)
            enemyActionText.text = string.Empty;
    }

    // ─────────── Sarsılma Efektleri ───────────

    /// <summary>Oyuncunun can barını sars.</summary>
    public void ShakePlayerHP()
    {
        if (playerHPShake != null) playerHPShake.Shake();
    }

    /// <summary>Oyuncunun can barını hasar oranına göre sars.</summary>
    public void ShakePlayerHP(float normalizedDamage)
    {
        if (playerHPShake != null) playerHPShake.Shake(normalizedDamage);
    }

    /// <summary>Düşmanın can barını sars.</summary>
    public void ShakeEnemyHP()
    {
        if (enemyHPShake != null) enemyHPShake.Shake();
    }

    /// <summary>Düşmanın can barını hasar oranına göre sars.</summary>
    public void ShakeEnemyHP(float normalizedDamage)
    {
        if (enemyHPShake != null) enemyHPShake.Shake(normalizedDamage);
    }
}
