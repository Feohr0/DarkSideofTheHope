using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Paneller (Canvas)")]
    public GameObject mapCanvas;     // Harita UI'ı
    public GameObject battleCanvas;  // Savaş HUD ve El UI'ı

    [Header("Bağımlılıklar")]
    public TurnManager turnManager;
    public DeckData playerMainDeck;  // Oyuncunun asıl destesi

    void Start()
    {
        // Oyun başladığında Haritayı aç, Savaşı gizle
        ShowMap();
    }

    public void ShowMap()
    {
        battleCanvas.SetActive(false);
        mapCanvas.SetActive(true);
    }
    
    public void StartEncounter(EncounterData encounterData)
    {
        // 1. Haritayı kapat, Savaşı aç
        mapCanvas.SetActive(false);
        battleCanvas.SetActive(true);

        // 2. TurnManager'a düşman verisini yolla ve savaşı başlat
        turnManager.InitBattle(playerMainDeck, encounterData);
    }

    public void EndBattle(bool playerWon)
    {
        ShowMap();

        if (playerWon)
            Debug.Log("Savaş kazanıldı! Haritada bir sonraki adıma geçebilirsin.");
        else
            Debug.Log("Oyun Bitti! Kaybettin.");
    }
}