using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Üst/alt HUD: can barı, enerji, tur yazısı
public class HUDView : MonoBehaviour
{
    [Header("Oyuncu")]
    public Slider            playerHPBar;
    public TextMeshProUGUI   playerHPText;
    public TextMeshProUGUI   playerEnergyText;
    public TextMeshProUGUI   playerShieldText;

    [Header("Düşman")]
    public Slider            enemyHPBar;
    public TextMeshProUGUI   enemyHPText;
    public TextMeshProUGUI   enemyShieldText;

    [Header("Genel")]
    public TextMeshProUGUI   turnText;
    public Button            endTurnButton;
    public TextMeshProUGUI   logText;          // Son olay mesajı

    private int playerMaxHP;
    private int enemyMaxHP;

    [Header("Ekonomi UI")]
    public TextMeshProUGUI goldText;

    // UIManager.cs içindeki Refresh fonksiyonunda bunu çağırabilirsin
    public void UpdateGold(int currentGold)
    {
        if (goldText != null) goldText.text = $"RUH PUANI: {currentGold}";
    }
    
    public void Init(int pMaxHP, int eMaxHP)
    {
        playerMaxHP = pMaxHP;
        enemyMaxHP  = eMaxHP;
        
        Debug.Log(playerMaxHP + eMaxHP);
    }

    public void UpdatePlayer(Player p)
    {
        // Artık maxHealth değerini direkt p'den çekiyoruz!
        playerHPBar.value     = (float)p.health / p.maxHealth;
        playerHPText.text     = $"{p.health} / {p.maxHealth}";
        
        playerEnergyText.text = $"mana: {p.currentEnergy} / {p.maxEnergy}";
        playerShieldText.text = p.shield > 0 ? $"kalkan: {p.shield}" : "";
    }

    public void UpdateEnemy(Player e)
    {
        // Aynı şekilde düşman için de:
        enemyHPBar.value     = (float)e.health / e.maxHealth;
        enemyHPText.text     = $"{e.health} / {e.maxHealth}";
        
        enemyShieldText.text = e.shield > 0 ? $"kalkan: {e.shield}" : "";
    }

    public void SetTurnText(string actorName)
        => turnText.text = $"Sıra {actorName}da";

    public void SetEndTurnInteractable(bool state)
        => endTurnButton.interactable = state;

    public void ShowLog(string message)
        => logText.text = message;
}