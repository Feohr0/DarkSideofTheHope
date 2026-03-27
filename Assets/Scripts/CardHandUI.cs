// Scripts/UI/CardHandUI.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kartları 3D world space'te yay şeklinde dizer.
/// Player'ın önünde birinci şahıs perspektifinde görünür.
/// </summary>
public class CardHandUI : MonoBehaviour
{
    public static CardHandUI Instance { get; private set; }

    [Header("Layout")]
    [SerializeField] private float cardSpacing  = 0.22f;
    [SerializeField] private float arcHeight    = 0.08f;
    [SerializeField] private float arcRotation  = 6f;
    [SerializeField] private float handDistance = 1.8f;
    [SerializeField] private float handYOffset  = -0.55f;

    [Header("Refs")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform  handAnchor;

    private readonly List<CardView3D> _cardViews = new();
    private CardView3D _hoveredCard;

    private DeckManager _deck;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _deck = Player.Instance.GetComponent<DeckManager>();
        EventBus.Subscribe<CardDrawnEvent>(OnCardDrawn);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<CardDrawnEvent>(OnCardDrawn);
    }

    private void OnCardDrawn(CardDrawnEvent evt)
    {
        SpawnCardView(evt.Card);
        ArrangeCards();
    }

    private void SpawnCardView(CardData data)
    {
        var go   = Instantiate(cardPrefab, handAnchor.position, handAnchor.rotation);
        var view = go.GetComponent<CardView3D>();
        view.Initialize(data, this);
        _cardViews.Add(view);
    }

    /// <summary>
    /// Kartları yay formasyonuna yerleştirir.
    /// </summary>
    public void ArrangeCards(CardView3D ignoredCard = null)
    {
        int count = _cardViews.Count;
        if (count == 0) return;

        float totalWidth = (count - 1) * cardSpacing;

        for (int i = 0; i < count; i++)
        {
            if (_cardViews[i] == ignoredCard) continue;

            float t      = count > 1 ? i / (float)(count - 1) : 0.5f;
            float xOff   = Mathf.Lerp(-totalWidth / 2f, totalWidth / 2f, t);
            float yOff   = -arcHeight * 4f * (t - 0.5f) * (t - 0.5f) + handYOffset;
            float rotZ   = Mathf.Lerp(arcRotation, -arcRotation, t);

            Vector3 targetPos = handAnchor.position
                + handAnchor.right   * xOff
                + handAnchor.up      * yOff
                + handAnchor.forward * handDistance;

            Quaternion targetRot = handAnchor.rotation
                * Quaternion.Euler(0, 0, rotZ);

            _cardViews[i].SetTargetTransform(targetPos, targetRot, i);
        }
    }

    public void OnCardHoverEnter(CardView3D card)
    {
        _hoveredCard = card;
        // Hover'daki kartı öne çıkar
        card.SetTargetTransform(
            card.TargetPosition + handAnchor.up * 0.15f,
            card.TargetRotation,
            999);
    }

    public void OnCardHoverExit(CardView3D card)
    {
        if (_hoveredCard == card) _hoveredCard = null;
        ArrangeCards();
    }

    public void OnCardPlayed(CardView3D cardView, Entity target)
    {
        bool success = _deck.TryPlayCard(cardView.Data, target);
        if (!success) return;

        _cardViews.Remove(cardView);
        Destroy(cardView.gameObject);
        ArrangeCards();
    }

    public void ClearHand()
    {
        foreach (var v in _cardViews)
            if (v != null) Destroy(v.gameObject);
        _cardViews.Clear();
    }
}