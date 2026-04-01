using UnityEngine;

// TurnManager ile View katmanını birbirine bağlar
public class UIManager : MonoBehaviour
{
    [Header("Bağımlılıklar")]
    public TurnManager turnManager;
    public HandView    handView;
    public HUDView     hudView;

    private void Start()
    {
        // HUD başlat
        hudView.Init(turnManager.player.health,
            turnManager.enemy.health);

        // "Turu Bitir" butonu
        hudView.endTurnButton.onClick.AddListener(OnEndTurnClicked);

        // İlk durumu çiz
        Refresh();
    }

    // TurnManager her değişiklikte bunu çağırır
    public void Refresh()
    {
        Player p = turnManager.player;
        Player e = turnManager.enemy;

        hudView.UpdatePlayer(p);
        hudView.UpdateEnemy(e);
        hudView.SetTurnText(turnManager.CurrentActorName);

        bool isPlayerTurn = turnManager.IsPlayerTurn;
        hudView.SetEndTurnInteractable(isPlayerTurn);

        // Eli sadece oyuncunun turunda çiz, düşmanın turunda temizle
        if (isPlayerTurn)
            handView.RenderHand(p.hand, p.currentEnergy, OnCardClicked);
        else
            handView.RenderHand(new System.Collections.Generic.List<Card>(),
                0, null);
    }

    private void OnCardClicked(Card card)
    {
        turnManager.TryPlayCard(card);

        // Kart oynandıktan sonra eli ve HUD'u güncelle
        hudView.UpdatePlayer(turnManager.player);
        hudView.UpdateEnemy(turnManager.enemy);
        handView.RefreshInteractability(turnManager.player.hand,
            turnManager.player.currentEnergy);

        hudView.ShowLog($"{card.cardName} oynandı!");
    }

    private void OnEndTurnClicked()
    {
        turnManager.EndTurn();
        Refresh();
    }
}