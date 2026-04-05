// Scripts/Gameplay/Level/LevelManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelManager
{   

    public static List<Tile> GenerateTiles(
        Transform grid,
        GameObject tilePrefab,
        List<TileDataSO> tileDatas,
        int spawnCount,
        float gridWidth = 4f,
        float gridHeight = 6f,
        float padding = 0.1f)
    {
        var result = new List<Tile>();
        if (tilePrefab == null || tileDatas == null || tileDatas.Count == 0)
            return result;

        var box = tilePrefab.GetComponent<BoxCollider2D>();
        var tileSize = box != null
            ? box.size + Vector2.one * padding
            : Vector2.one;

        var tiles = grid.GetComponentsInChildren<Tile>().ToList();

        for (int i = 0; i < spawnCount; i++)
        {
            // Đảm bảo spawn theo cặp 3 để luôn có thể match
            var data = tileDatas[Random.Range(0, tileDatas.Count)];
            int layer = GetRandomLayer(tiles);

            if (!TryFindPosition(tiles, tileSize, gridWidth, gridHeight, layer,
                    50, out Vector2 pos))
                continue;

            var tile = SpawnTile(grid, tilePrefab, data, pos, layer);
            if (tile == null) continue;

            result.Add(tile);
            tiles.Add(tile);
        }

        return result;
    }
    public static List<Tile> SpawnTiles(Transform parent, GameObject prefab, LevelDataSO level)
    {
        var result = new List<Tile>();

        foreach (var entry in level.tiles)
        {
            if (entry.tileData == null) continue;

            var go = Object.Instantiate(prefab, parent);
            go.transform.position = new Vector3(
                entry.position.x,
                entry.position.y,
                -entry.layer * 0.1f);   // layer cao hơn → z âm hơn → render trên

            var tile = go.GetComponent<Tile>();
            tile.Init(entry.tileData, entry.layer);
            result.Add(tile);
        }

        return result;
    }
    // ── Private ───────────────────────────────────────
    private static Tile SpawnTile(
        Transform grid, GameObject prefab,
        TileDataSO data, Vector2 pos, int layer)
    {
        var go = Object.Instantiate(prefab, grid);
        go.transform.position = pos;

        var tile = go.GetComponent<Tile>();
        if (tile == null) return null;

        tile.tileData = data;
        tile.layer = layer;
        tile.isBlocked = false;
        tile.isClicked = false;
        tile.worldPos = pos;
        tile.ApplyData();
        tile.UpdateSortingOrder();

        return tile;
    }

    private static int GetRandomLayer(List<Tile> tiles)
    {
        if (tiles.Count == 0) return 0;
        int maxLayer = tiles.Max(t => t.layer);
        return Random.Range(0, maxLayer + 2);
    }

    private static bool TryFindPosition(
        List<Tile> tiles, Vector2 tileSize,
        float gridWidth, float gridHeight,
        int layer, int maxAttempts, out Vector2 position)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(
                -gridWidth * 0.5f + tileSize.x * 0.5f,
                 gridWidth * 0.5f - tileSize.x * 0.5f);
            float y = Random.Range(
                -gridHeight * 0.5f + tileSize.y * 0.5f,
                 gridHeight * 0.5f - tileSize.y * 0.5f);

            var pos = new Vector2(x, y);

            if (!HasOverlapSameLayer(pos, tileSize, layer, tiles))
            {
                position = pos;
                return true;
            }
        }

        position = Vector2.zero;
        return false;
    }

    private static bool HasOverlapSameLayer(
        Vector2 pos, Vector2 size, int layer, List<Tile> tiles)
    {
        var newRect = new Rect(pos - size * 0.5f, size);

        foreach (var tile in tiles)
        {
            if (tile.layer != layer) continue;

            var tileRect = new Rect(
                (Vector2)tile.transform.position - size * 0.5f, size);

            if (newRect.Overlaps(tileRect)) return true;
        }

        return false;
    }
}