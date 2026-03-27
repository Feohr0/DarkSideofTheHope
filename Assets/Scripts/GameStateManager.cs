// Scripts/Core/GameStateManager.cs
using UnityEngine;

public enum GameState
{
    MainMenu, Map, Combat, Reward, Shop, Rest, GameOver, Victory
}

/// <summary>
/// State machine. Her state kendi enter/exit mantığını yönetir.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionTo(GameState newState)
    {
        OnExitState(CurrentState);
        CurrentState = newState;
        OnEnterState(newState);
        Debug.Log($"[State] → {newState}");
    }

    private void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Combat:
                CombatManager.Instance.StartCombat();
                break;
            case GameState.Map:
                MapGenerator.Instance.ShowMap();
                break;
            case GameState.GameOver:
                UIManager.Instance.ShowGameOver();
                break;
        }
    }

    private void OnExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Combat:
                CombatManager.Instance.CleanupCombat();
                break;
        }
    }
}