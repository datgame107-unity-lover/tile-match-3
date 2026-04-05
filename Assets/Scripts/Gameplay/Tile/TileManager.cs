// Scripts/Gameplay/Tile/TileManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardContext
{
    public Transform Root;
    public GameObject TilePrefab;

    public readonly List<Tile> CurrentTiles = new();
    public readonly List<Tile> SelectingTiles = new();
    public int MaxSelectableTile;
    public MonoBehaviour CoroutineRunner;
    public IGameModeHandler GameMode;
    public TileInputSystem InputSystem;
    public TileSelectionSystem TileSelectionSystem;
    public TileMatchSystem TileMatchSystem;
    public TileUtilitySystem TileUtilitySystem;
    public TileManager Manager;
}
// Scripts/Gameplay/Tile/TileManager.cs

public class TileManager : MonoBehaviour
{
    [Header("Board")]
    public GameObject tilePrefab;

    [Header("Settings")]
    public int maxSelectableTile = 9;

    private GameStateService gameState;
    private BoardContext context;

    private void Awake()
    {
        gameState = ServiceLocator.Get<GameStateService>();
        context = new BoardContext
        {
            MaxSelectableTile = maxSelectableTile,
            Root = transform,
            TilePrefab = tilePrefab,
            CoroutineRunner = this,
            Manager = this
        };
        context.InputSystem = new TileInputSystem(context);
        context.TileSelectionSystem = new TileSelectionSystem(context);
        context.TileMatchSystem = new TileMatchSystem(context);
        context.TileUtilitySystem = new TileUtilitySystem(context);

        var inputRunner = gameObject.AddComponent<TileInputRunner>();
        inputRunner.Initialize(context);

        IGameModeHandler handler = SceneLoader.PendingMode switch
        {
            GameMode.Level => new LevelModeHandler(context),
            GameMode.EndLess => new EndlessModeHandler(context),
            _ => null
        };
        SetModeHandler(handler);

        EventBus<PlayOnEvent>.Subscribe(OnPlayOn);
        EventBus<ContinueLevelEvent>.Subscribe(OnContinueLevel);
    }

    private void OnDestroy()
    {
        EventBus<PlayOnEvent>.Unsubscribe(OnPlayOn);
        EventBus<ContinueLevelEvent>.Unsubscribe(OnContinueLevel);
    }

    // ── Event handlers ────────────────────────────────

    private void OnPlayOn(PlayOnEvent _) => context.GameMode?.OnPlayOn();
    private void OnContinueLevel(ContinueLevelEvent _) => context.GameMode?.OnContinueLevel();

    // ── Mode ──────────────────────────────────────────

    public void SetModeHandler(IGameModeHandler handler)
    {   
        context.GameMode = handler;
        context.GameMode.Initialize();
    }

    // ── Public API ────────────────────────────────────

    public void SelectTile(Tile tile)
    {
        context.GameMode?.OnTileSelected(tile);
    }

    public void DefaultSelectLogic(Tile tile)
    {
        context.TileSelectionSystem.Select(tile);
        context.TileMatchSystem.CheckMatch();
        context.GameMode?.OnWinCheck(context.CurrentTiles, context.SelectingTiles);
    }

    public void PowerUp() => context.TileUtilitySystem.PowerUp();
    public void Shuffle() => context.TileUtilitySystem.Shuffle();
    public void Hint() => context.TileUtilitySystem.Hint();
    public void Undo() => context.TileUtilitySystem.Undo();

    public void RefreshCurrentTilesFromHierarchy()
    {
        context.CurrentTiles.Clear();
        context.CurrentTiles.AddRange(GetComponentsInChildren<Tile>());
    }

    public IEnumerator Win()
    {
        yield return new WaitForSeconds(0.3f);
        ServiceLocator.Get<GameStateService>().ChangeState(GameState.Win);
    }

    public void RegisterTile(Tile tile) => context.CurrentTiles.Add(tile);
    public void UnregisterTile(Tile tile) => context.CurrentTiles.Remove(tile);
}
