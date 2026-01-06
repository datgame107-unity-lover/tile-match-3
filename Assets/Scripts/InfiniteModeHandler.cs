using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfiniteModeHandler : IGameModeHandler
{
    private TileManager manager;
    private DangerSystem dangerSystem;
    private bool isSpawning = false;
    private float spawnInterval = 3f;

    public float comboMaxTime = 5f;
    private int comboCount = 0;
    private TileDataSO lastMatchData = null;
    private Coroutine comboTimerCor;

    public float aoeRadius = 1.2f;
    public GameObject explodeParticlePrefab;
    public InfiniteModeHandler(TileManager manager)
    {
        this.manager = manager;
    }
    public void Initialize()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            manager.StartCoroutine(SpawnTilesRoutine());
        }

        DangerBarUI dangerUI = GameObject.FindFirstObjectByType<DangerBarUI>();
        if (dangerUI == null)
        {
            Debug.LogError("[InfiniteMode] DangerBarUI NOT FOUND");
            return;
        }

        dangerSystem = new DangerSystem(dangerUI);
        dangerSystem.OnDangerMax += OnDangerMaxed;

        manager.StartCoroutine(DangerTickRoutine());
    }

    private IEnumerator SpawnTilesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            List<Tile> oldTiles = manager.transform.GetComponentsInChildren<Tile>().ToList();
         

            int spawnCount = Random.Range(15, 20);
            List<Tile> newTiles = LevelManager.GenerateTiles(manager.transform,manager.tilePrefab, manager.tileDatas, spawnCount);
            manager.currentTiles = manager.currentTiles.Concat(newTiles).ToList();
            dangerSystem.IncreaseBySpawn(); 
        }
    }

    public void OnTileSelected(Tile tile)
    {
        manager.DefaultSelectLogic(tile);
    }

    public void OnTilesMatched(TileDataSO data, Tile tile)
    {
        comboCount++;

        float reduceAmount = 0.05f + comboCount * 0.02f;
        dangerSystem.Decrease(reduceAmount);

        if (comboTimerCor != null)
            manager.StopCoroutine(comboTimerCor);

        comboTimerCor = manager.StartCoroutine(ComboTimer());

        lastMatchData ??= data;
        bool isSameType = lastMatchData == data;
        lastMatchData = data;

        // 🔔 BẮN EVENT COMBO
        EventManager.OnComboChanged?.Invoke(comboCount, 1f);
        EventManager.OnScoreAdd?.Invoke(comboCount);
        EventManager.OnTilesRemoved?.Invoke(data);

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


    private IEnumerator ComboTimer()
    {
        float timer = comboMaxTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            EventManager.OnComboChanged?.Invoke(comboCount, timer / comboMaxTime);
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
            manager.StopCoroutine(comboTimerCor);
            comboTimerCor = null;
        }

        EventManager.OnComboReset?.Invoke();
    }


    private void ClearAllSameType(TileDataSO data)
    {
        List<Tile> allTiles = manager.transform.GetComponentsInChildren<Tile>().ToList();

        foreach (Tile tile in allTiles)
        {
            if (tile.tileData == data)
                AnimateDestroy(tile);
        }

        Debug.Log("Clear all tile of type: " + data.name);
    }

    private void ExplodeAOE(Tile centerTile)
    {
        if (centerTile == null)
            return;

        Vector2 centerPos = centerTile.worldPos;

        // Spawn particle
        if (explodeParticlePrefab)
            GameObject.Instantiate(explodeParticlePrefab, centerPos, Quaternion.identity);

        List<Tile> allTiles = manager.transform.GetComponentsInChildren<Tile>().ToList();

        foreach (Tile tile in allTiles)
        {
            float dist = Vector2.Distance(tile.worldPos, centerPos);
            if (dist <= aoeRadius)
            {
                AnimateDestroy(tile);
            }
        }
    }

    private void AnimateDestroy(Tile tile)
    {
        Transform tr = tile.transform;

        Vector3 dir = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.3f, 1.3f),
            0
        ).normalized;

        float dist = Random.Range(0.6f, 1.4f);

        Sequence seq = DOTween.Sequence();
        seq.Append(tr.DOMove(tr.position + dir * dist, 0.45f).SetEase(Ease.OutQuad));
        seq.Join(tr.DOScale(0f, 0.45f));
        seq.Join(tr.DORotate(new Vector3(0, 0, Random.Range(180f, 720f)), 0.45f, RotateMode.FastBeyond360));
        seq.OnComplete(() => GameObject.Destroy(tile.gameObject));
    }

    public void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles)
    {
        // Infinite không có win
    }

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
        Debug.LogWarning("YOU LOST — Danger Maxed!");
        GameManager.instance.ChangeState(GameState.Lose);

        manager.StopAllCoroutines();
    }

    public void OnResetLevel()
    {
        InfiniteScoreManager.Instance.ResetScore();

        manager.selectingTiles = new List<Tile>();
        foreach(Tile t in manager.transform.GetComponentsInChildren<Tile>())
        {
            manager.currentTiles.Remove(t);
            GameObject.Destroy(t.gameObject);
        }    
        manager.StopAllCoroutines();
        dangerSystem.ResetValue();
        
        manager.StartCoroutine(SpawnTilesRoutine());
        manager.StartCoroutine(DangerTickRoutine());

    }

    public void OnPlayOn()
    {
        
    }

    public void OnContinueLevel()
    {
        throw new System.NotImplementedException();
    }
}
