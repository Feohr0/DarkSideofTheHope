using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Her kart prefab'ına bu script eklenir
public class CardView : MonoBehaviour
{
    [Header("Renkler")]
    public Color damageColor = new Color(0.85f, 0.2f, 0.2f);
    public Color shieldColor = new Color(0.2f, 0.5f, 0.85f);
    public Color healColor   = new Color(0.2f, 0.75f, 0.3f);

    private Card       cardData;
    private Action<Card> onClickCallback;

    [Header("UI Elemanları")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public TextMeshProUGUI flavorText;   // YENİ
    public Image           cardArt;      // YENİ
    public Image           cardBackground;
    public Button          cardButton;

    public void Setup(Card card, Action<Card> onClick)
    {
        cardData        = card;
        onClickCallback = onClick;

        nameText.text   = card.cardName;
        costText.text   = card.energyCost.ToString();
        powerText.text  = "GUC: " + card.power.ToString();
        effectText.text = " ";

        // Flavor & görsel (null güvenli)
        //if (flavorText != null) flavorText.text = card.flavorText;
        if (cardArt    != null) cardArt.sprite  = card.art;

        cardBackground.color = card.effect switch
        {
            Card.EffectType.Damage => damageColor,
            Card.EffectType.Shield => shieldColor,
            Card.EffectType.Heal   => healColor,
            _                      => Color.white
        };

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(() => onClickCallback?.Invoke(cardData));
    }

    // Kart oynanabilir mi? (enerji yeterliyse parlat, yetersizse soluklaştır)
    public void SetInteractable(bool interactable)
    {
        cardButton.interactable = interactable;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = interactable ? 1f : 0.45f;
    }
}