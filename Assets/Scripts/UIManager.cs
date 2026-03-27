// Scripts/UI/UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject combatHUDPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject rewardPanel;

    [Header("Combat HUD")]
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI drawPileCountText;
    [SerializeField] private TextMeshProUGUI discardPileCountText;
    [SerializeField] private Button endTurnButton;

    [Header("Player Stats")]
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TextMeshProUGUI blockText;

    [Header("Game Over")]
    [SerializeField] private TextMeshProUGUI gameOverFloorText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EventBus.Subscribe<EnergyChangedEvent>(OnEnergyChanged);
        EventBus.Subscribe<DamageDealtEvent>(OnDamageDealt);
        EventBus.Subscribe<TurnStartedEvent>(OnTurnStarted);

        endTurnButton.onClick.AddListener(() => CombatManager.Instance.OnEndTurnButtonPressed());
        restartButton.onClick.AddListener(OnRestartClicked);

        HideAllPanels();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EnergyChangedEvent>(OnEnergyChanged);
        EventBus.Unsubscribe<DamageDealtEvent>(OnDamageDealt);
        EventBus.Unsubscribe<TurnStartedEvent>(OnTurnStarted);
    }

    // --- Panel Yönetimi ---

    public void ShowCombatHUD()
    {
        HideAllPanels();
        combatHUDPanel.SetActive(true);
        RefreshPlayerStats();
    }

    public void ShowMap()
    {
        HideAllPanels();
        mapPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        HideAllPanels();
        gameOverPanel.SetActive(true);
        gameOverFloorText.text = "Kat: " + Player.Instance.Floor;
    }

    public void ShowVictory()
    {
        HideAllPanels();
        victoryPanel.SetActive(true);
    }

    public void ShowReward(System.Collections.Generic.List<CardData> rewardCards)
    {
        HideAllPanels();
        rewardPanel.SetActive(true);
        rewardPanel.GetComponent<RewardPanelUI>().Setup(rewardCards);
    }

    private void HideAllPanels()
    {
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        combatHUDPanel.SetActive(false);
        mapPanel.SetActive(false);
        rewardPanel.SetActive(false);
    }

    // --- Event Dinleyiciler ---

    private void OnEnergyChanged(EnergyChangedEvent evt)
    {
        energyText.text = evt.Current + " / " + evt.Max;
    }

    private void OnDamageDealt(DamageDealtEvent evt)
    {
        RefreshPlayerStats();
    }

    private void OnTurnStarted(TurnStartedEvent evt)
    {
        endTurnButton.interactable = evt.IsPlayerTurn;
        RefreshDeckCounts();
    }

    // --- Yardımcılar ---

    private void RefreshPlayerStats()
    {
        if (Player.Instance == null) return;

        playerHealthText.text = Player.Instance.CurrentHealth + " / " + Player.Instance.GetMaxHealth();
        playerHealthSlider.value = (float)Player.Instance.CurrentHealth / Player.Instance.GetMaxHealth();
        blockText.text = Player.Instance.CurrentBlock > 0
            ? Player.Instance.CurrentBlock.ToString()
            : "";
    }

    private void RefreshDeckCounts()
    {
        var deck = Player.Instance.GetComponent<DeckManager>();
        if (deck == null) return;

        drawPileCountText.text    = deck.DrawPile.Count.ToString();
        discardPileCountText.text = deck.DiscardPile.Count.ToString();
    }

    private void OnRestartClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}