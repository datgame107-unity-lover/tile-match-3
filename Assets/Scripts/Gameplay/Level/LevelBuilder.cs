// Scripts/Core/LevelBuilder.cs
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder
{
    // ── Build từ LevelRuntimeData (Level mode) ────────
    public List<Tile> Build(
        Transform parent,
        GameObject prefab,
        LevelRuntimeData level)
    {
        return TileSpawner.SpawnTiles(parent, prefab, level);
    }

    // ── Build random (Endless mode) ───────────────────
    public List<Tile> BuildRandom(
         Transform parent,
         GameObject prefab,
         TileDatabaseSO db,
         int count,
         GridConfig grid,
         List<Tile> existingTiles)
    {
        var result = new List<Tile>();
        var available = new List<Vector2>(grid.GetAllPositions());

        Shuffle(available);

        int maxAvailable = available.Count;
        int spawnCount = Mathf.Min(count, maxAvailable);
        spawnCount = (spawnCount / 3) * 3;

        if (spawnCount <= 0) return result;

        var tileDatas = PickRandomInTriples(db.tiles, spawnCount);

        // Tạo độ lệch ngẫu nhiên tối đa bằng kích thước collider
        float jitterOffset = grid.colliderSize * 0.25f;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-jitterOffset, jitterOffset),
                Random.Range(-jitterOffset, jitterOffset)
            );

            Vector2 spawnPos = available[i] + randomOffset;

            int layer = CalcLayer(spawnPos, existingTiles, result, grid.colliderSize);

            var tile = TileSpawner.Spawn(
                parent,
                prefab,
                tileDatas[i],
                spawnPos,
                layer);

            // (Tùy chọn) Xoay gạch một chút xíu cho giống một đống lộn xộn tự nhiên
            // tile.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));

            result.Add(tile);
        }

        var allTilesOnBoard = new List<Tile>(existingTiles);
        allTilesOnBoard.AddRange(result);
        RefreshAllBlocking(allTilesOnBoard, grid.colliderSize);

        return result;
    }

    // ── Blocking ─────────────────────────────────────

    private int CalcLayer(Vector2 pos, List<Tile> existing, List<Tile> newlySpawned, float colliderSize)
    {
        int max = 0;

        foreach (var tile in existing)
        {
            if (tile != null && IsOverlapping(pos, tile.transform.position, colliderSize))
                max = Mathf.Max(max, tile.layer + 1);
        }

        foreach (var tile in newlySpawned)
        {
            if (tile != null && IsOverlapping(pos, tile.transform.position, colliderSize))
                max = Mathf.Max(max, tile.layer + 1);
        }

        return max;
    }

    // CHUYỂN HÀM NÀY THÀNH PUBLIC để hệ thống khác có thể gọi lại khi nhặt gạch
    public void RefreshAllBlocking(List<Tile> tiles, float colliderSize)
    {
        foreach (var tile in tiles)
        {
            if (tile == null) continue;

            bool blocked = false;
            foreach (var other in tiles)
            {
                if (other == null || other == tile) continue;

                if (other.layer > tile.layer &&
                    IsOverlapping(tile.transform.position, other.transform.position, colliderSize))
                {
                    blocked = true;
                    break;
                }
            }
            tile.SetBlocked(blocked);
        }
    }

    private bool IsOverlapping(Vector2 a, Vector2 b, float colliderSize) =>
         Mathf.Abs(a.x - b.x) < colliderSize &&
         Mathf.Abs(a.y - b.y) < colliderSize;

    // ── Helpers ───────────────────────────────────────
    private List<TileDataSO> PickRandomInTriples(List<TileDataSO> source, int count)
    {
        var result = new List<TileDataSO>();
        int triples = count / 3;

        for (int i = 0; i < triples; i++)
        {
            var data = source[Random.Range(0, source.Count)];
            result.Add(data);
            result.Add(data);
            result.Add(data);
        }

        Shuffle(result);
        return result;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}