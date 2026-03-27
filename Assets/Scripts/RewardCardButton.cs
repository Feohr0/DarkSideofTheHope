// Scripts/UI/RewardCardButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Reward ekranında tek bir kart seçeneğini temsil eder.
/// Tıklanınca OnCardChosen callback'ini çağırır.
/// </summary>
public class RewardCardButton : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button          selectButton;
    [SerializeField] private RawImage        artworkImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image           rarityBorder;

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor   = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color uncommonColor = new Color(0.1f, 0.6f, 0.9f);
    [SerializeField] private Color rareColor     = new Color(0.9f, 0.7f, 0.1f);

    private CardData        _data;
    private Action<CardData> _onChosen;

    public void Setup(CardData data, Action<CardData> onChosen)
    {
        _data     = data;
        _onChosen = onChosen;

        nameText.text        = data.CardName;
        descriptionText.text = data.GetFormattedDescription();
        costText.text        = data.EnergyCost.ToString();

        if (data.Artwork != null)
            artworkImage.texture = data.Artwork.texture;

        if (rarityBorder != null)
            rarityBorder.color = GetRarityColor(data.Rarity);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        _onChosen?.Invoke(_data);
    }

    private Color GetRarityColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:   return commonColor;
            case CardRarity.Uncommon: return uncommonColor;
            case CardRarity.Rare:     return rareColor;
            default:                  return commonColor;
        }
    }
}
