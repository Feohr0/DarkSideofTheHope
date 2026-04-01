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
        if (hudView.endTurnButton != null)
        {
            hudView.endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
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
        // 1. Arka planda kartı oyna ve desteden düş
        turnManager.TryPlayCard(card);
    
        // 2. YANLIŞ YÖNTEM (Eskisi):
        // hudView.UpdatePlayer(turnManager.player);
        // hudView.UpdateEnemy(turnManager.enemy);
        // handView.RefreshInteractability(turnManager.player.hand, turnManager.player.currentEnergy);
    
        // DOĞRU YÖNTEM: Kart elden silindiği için tüm UI'ı ve eli baştan çizdir.
        Refresh(); 
    
        // 3. Log mesajını göster
        hudView.ShowLog($"{card.cardName} oynandı!");
    }

    private void OnEndTurnClicked()
    {
        turnManager.EndTurn();
        Refresh();
    }
}