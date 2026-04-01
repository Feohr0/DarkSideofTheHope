using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "CardGame/Deck")]
public class DeckData : ScriptableObject
{
    public string         deckName;
    public List<CardData> cards;    // Inspector'dan sürükle-bırak

    // Karıştırılmış runtime destesi döndür
    public List<Card> BuildShuffledDeck()
    {
        List<Card> deck = new List<Card>();

        foreach (CardData data in cards)
            deck.Add(data.ToCard());

        Shuffle(deck);
        return deck;
    }

    private void Shuffle(List<Card> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j    = Random.Range(0, i + 1);
            Card tmp = deck[i];
            deck[i]  = deck[j];
            deck[j]  = tmp;
        }
    }
}