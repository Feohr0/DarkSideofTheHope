using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string playerName;
    public int    health;
    public int    maxEnergy;   // her turun başında sıfırlanacak maksimum enerji
    public int    currentEnergy;

    public List<Card> hand  = new List<Card>();   // eldeki kartlar
    public List<Card> deck  = new List<Card>();   // çekilmemiş deste
    
    public int shield = 0; 
    
    public Player(string name, int hp, int maxEn)
    {
        playerName = name;
        health     = hp;
        maxEnergy  = maxEn;
        currentEnergy = maxEnergy;
    }

    // Tur başında enerjiyi doldur
    public void RefillEnergy() => currentEnergy = maxEnergy;

    // Desteden kart çek
    public void DrawCard()
    {
        if (deck.Count == 0) { Debug.Log("Deste bitti!"); return; }

        Card drawn = deck[0];
        deck.RemoveAt(0);
        hand.Add(drawn);
        Debug.Log($"{playerName} çekti: {drawn}");
    }

    // Kart oyna (enerji yeterliyse)
    public bool PlayCard(Card card)
    {
        if (!hand.Contains(card))        { Debug.Log("Kart elde yok.");           return false; }
        if (currentEnergy < card.energyCost) { Debug.Log("Yetersiz enerji.");     return false; }

        currentEnergy -= card.energyCost;
        hand.Remove(card);
        Debug.Log($"{playerName} oynadı: {card}  |  Kalan enerji: {currentEnergy}");
        return true;
    }

    // Hasar alırken kalkanı önce tüket
    public void TakeDamage(int amount)
    {
        int absorbed = Mathf.Min(shield, amount);
        shield  -= absorbed;
        amount  -= absorbed;
        health  -= amount;
        health   = Mathf.Max(health, 0);   // negatife düşmesin

        Debug.Log($"{playerName} → {absorbed} kalkanla engellendi, " +
                  $"{amount} hasar aldı | HP:{health}  Kalkan:{shield}");
    }

    public void AddShield(int amount)
    {
        shield += amount;
        Debug.Log($"{playerName} +{amount} kalkan kazandı | Kalkan:{shield}");
    }

    public void HealHealth(int amount)
    {
        health += amount;
        Debug.Log($"{playerName} +{amount} can kazandı | HP:{health}");
    }
    
    public bool IsAlive => health > 0;

    public override string ToString()
        => $"{playerName} — HP:{health}  Enerji:{currentEnergy}/{maxEnergy}  Elde:{hand.Count} kart";
}