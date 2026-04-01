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

    public void Init(int pMaxHP, int eMaxHP)
    {
        playerMaxHP = pMaxHP;
        enemyMaxHP  = eMaxHP;
    }

    public void UpdatePlayer(Player p)
    {
        playerHPBar.value    = (float)p.health / playerMaxHP;
        playerHPText.text    = $"{p.health} / {playerMaxHP}";
        playerEnergyText.text = $"⚡ {p.currentEnergy} / {p.maxEnergy}";
        playerShieldText.text = p.shield > 0 ? $"🛡 {p.shield}" : "";
    }

    public void UpdateEnemy(Player e)
    {
        enemyHPBar.value   = (float)e.health / enemyMaxHP;
        enemyHPText.text   = $"{e.health} / {enemyMaxHP}";
        enemyShieldText.text = e.shield > 0 ? $"🛡 {e.shield}" : "";
    }

    public void SetTurnText(string actorName)
        => turnText.text = $"{actorName}'ın Turu";

    public void SetEndTurnInteractable(bool state)
        => endTurnButton.interactable = state;

    public void ShowLog(string message)
        => logText.text = message;
}