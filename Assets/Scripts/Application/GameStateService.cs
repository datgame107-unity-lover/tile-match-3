using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class GameStateService
{
    public GameState CurrentState { get; private set; }
    public GameMode CurrentMode { get; private set; }
    public int CurrentLevel { get; private set; }

    private readonly ISaveService _saveService;
    public GameStateService(ISaveService save)
    {
        _saveService = save;
        CurrentLevel = _saveService.GetInt(SaveKeys.Player.Level, 1);
    }

    public void ChangeState(GameState state)
    {
        if (state == CurrentState)
        {
            return;
        }

        CurrentState = state;
        EventBus<GameStateChangedEvent>.Publish(
             new GameStateChangedEvent { state = state });

        switch (state)
        {
            case GameState.Win: HandleWin(); break;
            case GameState.Lose: HandleLose(); break;
        }

    }

    public void ChangeMode(GameMode mode)
    {
        CurrentMode = mode;
        EventBus<GameModeChangedEvent>.Publish( new GameModeChangedEvent { mode = mode });

    }
    public void SaveLevel( int level)
    {
        CurrentLevel = level;
        _saveService.SetInt(SaveKeys.Player.Level, level);
        _saveService.Save();
        EventBus<LevelSavedEvent>.Publish(new LevelSavedEvent());
    }
    private void HandleWin()
    {
        SaveLevel(CurrentLevel + 1);
        EventBus<PlayerWonEvent>.Publish(new PlayerWonEvent());
    }

    private void HandleLose()
    {
        EventBus<PlayerLostEvent>.Publish(new PlayerLostEvent());
    }
}
