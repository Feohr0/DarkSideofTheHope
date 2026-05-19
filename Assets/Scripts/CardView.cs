using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

// Her kart prefab'ına bu script eklenir
public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Renkler")]
    public Color damageColor = new Color(0.85f, 0.2f, 0.2f);
    public Color shieldColor = new Color(0.2f, 0.5f, 0.85f);
    public Color healColor   = new Color(0.2f, 0.75f, 0.3f);

    [Header("Hover Efekti")]
    public Vector3 hoverScale = new Vector3(1.15f, 1.15f, 1f);

    private Card       cardData;
    private Action<Card> onClickCallback;
    private Vector3 originalScale;

    [Header("UI Elemanları")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public TextMeshProUGUI flavorText;   // YENİ
    public Image           cardArt;      // YENİ
    public Image           cardBackground;
    public Button          cardButton;

    private GameObject tooltipRoot;
    private TextMeshProUGUI tooltipText;
    
    [Header("Random Kart Görselleri (Opsiyonel)")]
    public Sprite[] randomDamageArts; // saldırı
    public Sprite[] randomShieldArts; // kalkan

    public void Setup(Card card, Action<Card> onClick)
    {
        cardData        = card;
        onClickCallback = onClick;
        originalScale   = transform.localScale;

        nameText.text   = card.cardName;
        costText.text   = card.energyCost.ToString();
        powerText.text  = "Güç: " + card.power.ToString();
        effectText.text = " ";

        // Flavor & görsel (null güvenli)
        //if (flavorText != null) flavorText.text = card.flavorText;
        if (cardArt != null)
        {
            cardArt.sprite = PickRandomArtOrDefault(card);
        }

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

    public void SetHoverTooltip(GameObject tooltipObject, TextMeshProUGUI tooltipLabel)
    {
        tooltipRoot = tooltipObject;
        tooltipText = tooltipLabel;
    }
    
    private Sprite PickRandomArtOrDefault(Card card)
    {
        if (card == null) return null;

        Sprite[] pool = card.effect switch
        {
            Card.EffectType.Damage => randomDamageArts,
            Card.EffectType.Shield => randomShieldArts,
            _ => null
        };

        if (pool != null && pool.Length > 0)
        {
            Sprite chosen = pool[UnityEngine.Random.Range(0, pool.Length)];
            if (chosen != null) return chosen;
        }

        return card.art;
    }

    // Kart oynanabilir mi? (enerji yeterliyse parlat, yetersizse soluklaştır)
    public void SetInteractable(bool interactable)
    {
        cardButton.interactable = interactable;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = interactable ? 1f : 0.45f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipRoot == null || tooltipText == null || cardData == null || cardData.data == null) return;

        transform.localScale = hoverScale;
        tooltipText.text = cardData.data.hoverDescription;
        tooltipRoot.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void OnDisable()
    {
        HideTooltip();
    }

    private void HideTooltip()
    {
        transform.localScale = originalScale;

        if (tooltipRoot != null)
        {
            tooltipRoot.SetActive(false);
        }
    }
}
