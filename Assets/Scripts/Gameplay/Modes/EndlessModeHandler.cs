// Scripts/Gameplay/Modes/EndlessModeHandler.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EndlessModeHandler : IGameModeHandler
{
    private readonly BoardContext context;
    private readonly GameStateService gameState;
    private readonly ScoreService scoreService;

    private DangerSystem dangerSystem;

    // Spawn
    private bool isSpawning;
    private float spawnInterval = 3f;

    // Combo
    private int comboCount;
    private float comboMaxTime = 5f;
    private TileDataSO lastMatchData;
    private Coroutine comboTimerCor;
    private GridConfig gridConfig;
    // AOE
    private float aoeRadius = 1.2f;
    private GameObject explodeParticlePrefab;

    public EndlessModeHandler(BoardContext context)
    {
        this.context = context;
        gameState = ServiceLocator.Get<GameStateService>();
        scoreService = ServiceLocator.Get<ScoreService>();
        context.GameMode = this;
        gridConfig = ServiceLocator.Get<GridConfig>();
    }

    // ── IGameModeHandler ──────────────────────────────

    public void Initialize()
    {
        gameState.ChangeMode(GameMode.EndLess);

        dangerSystem = new DangerSystem();
        dangerSystem.OnDangerMax += OnDangerMaxed;

        if (!isSpawning)
        {
            isSpawning = true;
            context.CoroutineRunner.StartCoroutine(SpawnRoutine());
        }

        context.CoroutineRunner.StartCoroutine(DangerTickRoutine());
    }

    public void OnPlayOn()
    {
        gameState.ChangeState(GameState.Playing);
    }

    public void OnTileSelected(Tile tile)
    {
        context.Manager.DefaultSelectLogic(tile);
    }

    public void OnTilesMatched(TileDataSO data, Tile tile)
    {
        comboCount++;

        float reduce = 0.05f + comboCount * 0.02f;
        dangerSystem.Decrease(reduce);

        if (comboTimerCor != null)
            context.CoroutineRunner.StopCoroutine(comboTimerCor);
        comboTimerCor = context.CoroutineRunner.StartCoroutine(ComboTimerRoutine());

        bool isSameType = lastMatchData == data;
        lastMatchData = data;

        EventBus<ComboChangedEvent>.Publish(new ComboChangedEvent
        {
            count = comboCount,
            timeRatio = 1f,
        });
        EventBus<ScoreAddedEvent>.Publish(new ScoreAddedEvent { amount = comboCount });
        EventBus<TilesRemovedEvent>.Publish(new TilesRemovedEvent { tileData = data });

        if ((comboCount == 2 && isSameType) || comboCount == 7)
        {
            ClearAllSameType(data);
            ResetCombo();
            return;
        }

        if (comboCount == 3 && !isSameType)
        {
            ExplodeAOE(tile);
            return;
        }
    }

    public void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles)
    {
        // Endless không có win condition
    }

    public void OnResetLevel()
    {
        scoreService.ResetScore();
        ResetCombo();

        foreach (var tile in context.CurrentTiles)
            if (tile != null) Object.Destroy(tile.gameObject);

        context.CurrentTiles.Clear();
        context.SelectingTiles.Clear();

        context.CoroutineRunner.StopAllCoroutines();
        dangerSystem.Reset();

        isSpawning = true;
        context.CoroutineRunner.StartCoroutine(SpawnRoutine());
        context.CoroutineRunner.StartCoroutine(DangerTickRoutine());
    }

    public void OnContinueLevel()
    {
        gameState.ChangeState(GameState.Playing);
    }

    // ── Spawn ─────────────────────────────────────────

    private IEnumerator SpawnRoutine()
    {
        var db = ServiceLocator.Get<TileDatabaseSO>();
        var builder = ServiceLocator.Get<LevelBuilder>();

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int spawnCount = Random.Range(15, 20);

            var newTiles = builder.BuildRandom(
                context.Root,
                context.TilePrefab,
                db,
                spawnCount,
                gridConfig);

            context.CurrentTiles.AddRange(newTiles);
            dangerSystem.IncreaseBySpawn();
        }
    }

    // ── Danger ────────────────────────────────────────

    private IEnumerator DangerTickRoutine()
    {
        while (true)
        {
            dangerSystem.Tick(Time.deltaTime);
            yield return null;
        }
    }

    private void OnDangerMaxed()
    {
        gameState.ChangeState(GameState.Lose);
        context.CoroutineRunner.StopAllCoroutines();
    }

    // ── Combo ─────────────────────────────────────────

    private IEnumerator ComboTimerRoutine()
    {
        float timer = comboMaxTime;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            EventBus<ComboChangedEvent>.Publish(new ComboChangedEvent
            {
                count = comboCount,
                timeRatio = timer / comboMaxTime,
            });
            yield return null;
        }

        ResetCombo();
    }

    private void ResetCombo()
    {
        comboCount = 0;
        lastMatchData = null;

        if (comboTimerCor != null)
        {
            context.CoroutineRunner.StopCoroutine(comboTimerCor);
            comboTimerCor = null;
        }

        EventBus<ComboResetEvent>.Publish(new ComboResetEvent());
    }

    // ── Special effects ───────────────────────────────

    private void ClearAllSameType(TileDataSO data)
    {
        var toDestroy = context.CurrentTiles
            .Where(t => t != null && t.tileData == data)
            .ToList();

        foreach (var tile in toDestroy)
        {
            context.CurrentTiles.Remove(tile);
            context.SelectingTiles.Remove(tile);
            AnimateDestroy(tile);
        }
    }

    private void ExplodeAOE(Tile centerTile)
    {
        if (centerTile == null) return;

        Vector2 center = centerTile.worldPos;

        if (explodeParticlePrefab != null)
            Object.Instantiate(explodeParticlePrefab, center, Quaternion.identity);

        var toDestroy = context.CurrentTiles
            .Where(t => t != null &&
                        Vector2.Distance(t.worldPos, center) <= aoeRadius)
            .ToList();

        foreach (var tile in toDestroy)
        {
            context.CurrentTiles.Remove(tile);
            context.SelectingTiles.Remove(tile);
            AnimateDestroy(tile);
        }
    }

    private void AnimateDestroy(Tile tile)
    {
        var tr = tile.transform;
        var dir = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.3f, 1.3f),
            0f).normalized;

        float dist = Random.Range(0.6f, 1.4f);

        DOTween.Sequence()
            .Append(tr.DOMove(tr.position + dir * dist, 0.45f).SetEase(Ease.OutQuad))
            .Join(tr.DOScale(0f, 0.45f))
            .Join(tr.DORotate(
                new Vector3(0f, 0f, Random.Range(180f, 720f)),
                0.45f, RotateMode.FastBeyond360))
            .OnComplete(() => Object.Destroy(tile.gameObject));
    }
}