using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfiniteModeHandler : IGameModeHandler
{
    private TileManager manager;

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

        if (!isSpawning)
        {
            isSpawning = true;
            manager.StartCoroutine(SpawnTilesRoutine());
        }
    }

    private IEnumerator SpawnTilesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            List<Tile> oldTiles = manager.transform.GetComponentsInChildren<Tile>().ToList();
            foreach (Tile t in oldTiles)
                t.AddLayer(1);

            int spawnCount = Random.Range(10, 15);
            List<Tile> newTiles = LevelManager.GenerateTiles(manager.transform, manager.tileDatas, spawnCount);
            List<Tile> allTiles = oldTiles.Concat(newTiles).ToList();

            manager.SortTileAndActivateShadow(allTiles);
        }
    }

    public void OnTileSelected(Tile tile)
    {
        manager.DefaultSelectLogic(tile);
    }

    public void OnTilesMatched(TileDataSO data, Tile tile)
    {
        comboCount++;

        // reset timer
        if (comboTimerCor != null)
            manager.StopCoroutine(comboTimerCor);
        comboTimerCor = manager.StartCoroutine(ComboTimer());

        bool isSameType = (lastMatchData == data);
        lastMatchData = data;
        EventManager.OnTilesRemoved?.Invoke(data);
        Debug.Log("Combo = " + comboCount + " sameType = " + isSameType);

        // --- RULE 1: nếu 3 cái cùng loại → clear hết loại đó ---
        if ((comboCount == 2 && isSameType)||comboCount==7)
        {
            ClearAllSameType(data);
            ResetCombo();
            return;
        }

        // --- RULE 2: nếu 3 cái nhưng KHÁC loại → nổ AOE ---
        if (comboCount == 3 && !isSameType)
        {
            ExplodeAOE(tile);
            return;
        }
    }

    private IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(comboMaxTime);
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

        Debug.Log("Combo Reset");
    }

    // ============================
    // RULE 1: Clear toàn bộ tile cùng loại
    // ============================
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

    // ============================
    // RULE 2: Nổ AOE theo bán kính
    // ============================
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

    // ============================
    // Animation phá tile
    // ============================
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
}
