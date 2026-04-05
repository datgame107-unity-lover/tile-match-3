// Scripts/Gameplay/Modes/LevelModeHandler.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModeHandler : IGameModeHandler
{
    private readonly LevelBuilder levelBuilder;
    private readonly BoardContext context;
    private readonly GameStateService gameState;
    private readonly ISaveService save;
    private readonly LevelDataManager levelDataManager;

    public LevelModeHandler(BoardContext context)
    {   
        this.context = context;

        gameState = ServiceLocator.Get<GameStateService>();
        save = ServiceLocator.Get<ISaveService>();
        levelDataManager = ServiceLocator.Get<LevelDataManager>();
        levelBuilder = ServiceLocator.Get<LevelBuilder>();

        context.GameMode = this;
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────
    public void Initialize()
    {
        gameState.ChangeMode(GameMode.Level);
        SpawnLevel();
    }

    public void OnPlayOn()
    {
        gameState.ChangeState(GameState.Playing);
    }

    // ─────────────────────────────────────────────
    // Tile Interaction
    // ─────────────────────────────────────────────
    public void OnTileSelected(Tile tile)
    {
        context.TileSelectionSystem.Select(tile);
        context.TileMatchSystem.CheckMatch();

        OnWinCheck(context.CurrentTiles, context.SelectingTiles);
    }

    public void OnTilesMatched(TileDataSO tileData, Tile tile)
    {
        // handled elsewhere (ex: EventBus → CurrencyService)
    }

    public void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles)
    {
        Debug.Log(currentTiles.Count);
        if (currentTiles.Count == 0 && selectingTiles.Count == 0)
        {
            context.CoroutineRunner.StartCoroutine(WinRoutine());
        }
    }

    // ─────────────────────────────────────────────
    // Level Control
    // ─────────────────────────────────────────────
    public void OnResetLevel()
    {
        // destroy old tiles
        foreach (var tile in context.CurrentTiles)
        {
            if (tile != null)
                Object.Destroy(tile.gameObject);
        }

        context.CurrentTiles.Clear();
        context.SelectingTiles.Clear();

        SpawnLevel();
        gameState.ChangeState(GameState.Playing);
    }

    public void OnContinueLevel()
    {
        // keep board, clear hand only
        context.SelectingTiles.Clear();

        RefreshTilesFromHierarchy();
        SpawnLevel();
        gameState.ChangeState(GameState.Playing);
    }

    // ─────────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────────
    private void SpawnLevel()
    {
        var levelManager = ServiceLocator.Get<LevelDataManager>();
        var db = ServiceLocator.Get<TileDatabaseSO>();
        var builder = ServiceLocator.Get<LevelBuilder>();

        int levelIndex = save.GetInt(SaveKeys.Player.Level,1);

        // Nếu chưa có save, default về level đầu tiên
        if (levelIndex <= 0) levelIndex = 1;

        var savel = levelManager.GetLevel(levelIndex);
        var runtime = LevelMapper.FromSave(savel, db);

        var tiles = builder.Build(
            context.Root,
            context.TilePrefab,
            runtime);
        context.CurrentTiles.AddRange(tiles);
    }

    private void RefreshTilesFromHierarchy()
    {
        context.CurrentTiles.Clear();

        var tiles = context.Root.GetComponentsInChildren<Tile>();
        context.CurrentTiles.AddRange(tiles);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        gameState.ChangeState(GameState.Win);
        Debug.Log(gameState.CurrentState);
    }
}
