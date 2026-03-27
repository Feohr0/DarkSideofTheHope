// Scripts/Entities/Player.cs
using UnityEngine;

public class Player : Entity
{
    public static Player Instance { get; private set; }

    [Header("Energy")]
    [SerializeField] private int maxEnergy = 3;
    public int CurrentEnergy { get; private set; }

    [Header("Run Data")]
    public int Gold { get; private set; }
    public int Floor { get; private set; }

    private DeckManager _deckManager;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _deckManager = GetComponent<DeckManager>();
    }

    public void StartCombatTurn()
    {
        ResetBlock();
        CurrentEnergy = maxEnergy;
        EventBus.Publish(new EnergyChangedEvent(CurrentEnergy, maxEnergy));
        ProcessEffectsOnTurnStart();
        _deckManager.DrawCards(5);
    }

    public bool TrySpendEnergy(int cost)
    {
        if (CurrentEnergy < cost) return false;
        CurrentEnergy -= cost;
        EventBus.Publish(new EnergyChangedEvent(CurrentEnergy, maxEnergy));
        return true;
    }

    public void GainGold(int amount) => Gold += amount;
    public void SpendGold(int amount) => Gold = Mathf.Max(0, Gold - amount);
    public void IncrementFloor() => Floor++;

    private void ResetBlock() => CurrentBlock = 0;

    protected override void OnDeath()
    {
        base.OnDeath();
        GameStateManager.Instance.TransitionTo(GameState.GameOver);
    }
}