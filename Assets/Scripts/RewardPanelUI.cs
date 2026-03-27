// Scripts/UI/RewardPanelUI.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Savaş sonrası kart seçim ekranı.
/// </summary>
public class RewardPanelUI : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject rewardCardButtonPrefab;
    [SerializeField] private TextMeshProUGUI goldRewardText;
    [SerializeField] private UnityEngine.UI.Button skipButton;

    public void Setup(List<CardData> cards)
    {
        // Eski butonları temizle
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        // Kart butonlarını oluştur
        foreach (var card in cards)
        {
            var go     = Instantiate(rewardCardButtonPrefab, cardContainer);
            var btn    = go.GetComponent<RewardCardButton>();
            btn.Setup(card, OnCardChosen);
        }

        // Altın ödülü göster
        int gold = Random.Range(10, 25) + Player.Instance.Floor * 2;
        goldRewardText.text = "+" + gold + " Altın";
        Player.Instance.GainGold(gold);

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(OnSkip);
    }

    private void OnCardChosen(CardData card)
    {
        RunManager.Instance.AddCardToDeck(card);
        GameStateManager.Instance.TransitionTo(GameState.Map);
    }

    private void OnSkip()
    {
        GameStateManager.Instance.TransitionTo(GameState.Map);
    }
}