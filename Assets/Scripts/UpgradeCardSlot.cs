using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardSlot : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Button selectButton;
    public Image cardArtImage;
    public Image selectionFrame;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI summaryText;

    [Header("Renkler")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.9f, 0.45f, 1f);

    private CardData boundCard;

    public CardData BoundCard => boundCard;

    public void Setup(CardData data, bool isSelected, Action<CardData> onSelect)
    {
        boundCard = data;

        if (nameText != null)
        {
            nameText.text = data != null ? data.cardName : "-";
        }

        if (levelText != null)
        {
            levelText.text = data != null
                ? $"Sv. {data.currentLevel}/{CardData.MaxUpgradeLevel}"
                : string.Empty;
        }

        if (summaryText != null)
        {
            summaryText.text = data != null
                ? $"Güç {data.GetCurrentPower()} | Enerji {data.GetCurrentCost()}"
                : string.Empty;
        }

        if (cardArtImage != null)
        {
            cardArtImage.sprite = data != null ? data.cardArt : null;
            cardArtImage.enabled = data != null && data.cardArt != null;
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect?.Invoke(boundCard));
            selectButton.interactable = data != null;
        }

        SetSelected(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionFrame != null)
        {
            selectionFrame.color = isSelected ? selectedColor : normalColor;
        }
    }
}
