// Scripts/Gameplay/Input/TileInputRunner.cs
using UnityEngine;

public class TileInputRunner : MonoBehaviour
{
    private BoardContext context;
    private GameStateService gameState;

    public void Initialize(BoardContext ctx)
    {
        context = ctx;
        gameState = ServiceLocator.Get<GameStateService>();
    }

    private void Update()
    {
        if (gameState.CurrentState != GameState.Playing)
            return;

        context.InputSystem.HandleInput();
    }
}