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
        GridConfig grid)
    {
        var result = new List<Tile>();
        var available = new List<Vector2>(grid.GetAllPositions());

        count = (count / 3) * 3;

        Shuffle(available);
        int spawnCount = Mathf.Min(count, available.Count);

        var tileDatas = PickRandomInTriples(db.tiles, spawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            // tính layer dựa vào tile đã spawn (giống BoardController.CalcLayer)
            int layer = CalcLayer(available[i], result, grid.colliderSize);

            var tile = TileSpawner.Spawn(
                parent,
                prefab,
                tileDatas[i],
                available[i],
                layer);

            result.Add(tile);
        }

        // refresh blocking + shadow cho toàn bộ tile vừa spawn
        RefreshAllBlocking(result, grid.colliderSize);

        return result;
    }

    // ── Blocking ─────────────────────────────────────

    private int CalcLayer(Vector2 pos, List<Tile> existing, float colliderSize)
    {
        int max = 0;
        foreach (var tile in existing)
        {
            if (IsOverlapping(pos, tile.transform.position, colliderSize))
                max = Mathf.Max(max, tile.layer + 1);
        }
        return max;
    }

    private void RefreshAllBlocking(List<Tile> tiles, float colliderSize)
    {
        foreach (var tile in tiles)
        {
            bool blocked = false;
            foreach (var other in tiles)
            {
                if (other == tile) continue;
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