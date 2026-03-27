// Scripts/Cards/DeckManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deck, hand ve discard pile yönetimi.
/// Karıştırma ve çekme işlemleri burada.
/// </summary>
public class DeckManager : MonoBehaviour
{
    private List<CardData> _drawPile    = new();
    private List<CardData> _hand        = new();
    private List<CardData> _discardPile = new();
    private List<CardData> _exhaustPile = new();

    public IReadOnlyList<CardData> Hand        => _hand;
    public IReadOnlyList<CardData> DrawPile    => _drawPile;
    public IReadOnlyList<CardData> DiscardPile => _discardPile;

    public int HandSize => _hand.Count;

    public void InitializeDeck(List<CardData> cards)
    {
        _drawPile.Clear();
        _hand.Clear();
        _discardPile.Clear();
        _drawPile.AddRange(cards);
        Shuffle(_drawPile);
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (_drawPile.Count == 0)
                ReshuffleDiscardIntoDraw();

            if (_drawPile.Count == 0) break; // Gerçekten boş

            var card = _drawPile[0];
            _drawPile.RemoveAt(0);
            _hand.Add(card);
            EventBus.Publish(new CardDrawnEvent(card));
        }
    }

    public bool TryPlayCard(CardData card, Entity target)
    {
        if (!_hand.Contains(card)) return false;
        if (!Player.Instance.TrySpendEnergy(card.EnergyCost)) return false;

        _hand.Remove(card);
        CardEffectResolver.Instance.Resolve(card, Player.Instance, target);
        _discardPile.Add(card);
        return true;
    }

    public void ExhaustCard(CardData card)
    {
        _hand.Remove(card);
        _exhaustPile.Add(card);
    }

    public void DiscardHand()
    {
        _discardPile.AddRange(_hand);
        _hand.Clear();
    }

    private void ReshuffleDiscardIntoDraw()
    {
        _drawPile.AddRange(_discardPile);
        _discardPile.Clear();
        Shuffle(_drawPile);
        Debug.Log("[Deck] Discard reshuffle edildi.");
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}