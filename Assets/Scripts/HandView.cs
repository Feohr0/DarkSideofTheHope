using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Oyuncunun elindeki kartları UI'a yansıtır
public class HandView : MonoBehaviour
{
    public GameObject  cardPrefab;     // CardView prefab'ı
    public Transform   handContainer;  // Horizontal Layout Group'un içi
    public GameObject  hoverInfoRoot;
    public TextMeshProUGUI hoverInfoText;

    private List<GameObject> spawnedCards = new List<GameObject>();

    private void Awake()
    {
        if (hoverInfoRoot != null)
            hoverInfoRoot.SetActive(false);
    }

    // Eli sıfırdan çiz
    public void RenderHand(List<Card> hand, int currentEnergy,
        System.Action<Card> onCardClicked)
    {
        ClearHand();

        foreach (Card card in hand)
        {
            GameObject go   = Instantiate(cardPrefab, handContainer);
            CardView   view = go.GetComponent<CardView>();

            view.Setup(card, onCardClicked);
            view.SetHoverTooltip(hoverInfoRoot, hoverInfoText);
            view.SetInteractable(card.energyCost <= currentEnergy);

            spawnedCards.Add(go);
        }
    }

    // Enerji değişince kartların interactable durumunu güncelle
    public void RefreshInteractability(List<Card> hand, int currentEnergy)
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            CardView view = spawnedCards[i].GetComponent<CardView>();
            view.SetInteractable(hand[i].energyCost <= currentEnergy);
        }
    }

    private void ClearHand()
    {
        if (hoverInfoRoot != null)
            hoverInfoRoot.SetActive(false);

        foreach (GameObject go in spawnedCards)
            Destroy(go);
        spawnedCards.Clear();
    }
}
