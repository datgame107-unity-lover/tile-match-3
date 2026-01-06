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
        List<Tile> result = new();
        if (tilePrefab == null || tileDatas == null || tileDatas.Count == 0)
            return result;

        BoxCollider2D box = tilePrefab.GetComponent<BoxCollider2D>();
        Vector2 tileSize = box != null
            ? box.size + Vector2.one * padding
            : Vector2.one;

        List<Tile> tiles = grid.GetComponentsInChildren<Tile>().ToList();
        const int MAX_ATTEMPTS = 50;

        for (int i = 0; i < spawnCount; i++)
        {
            TileDataSO data = tileDatas[UnityEngine.Random.Range(0, tileDatas.Count)];
            int layer = GetRandomLayer(tiles);

            if (!TryFindSpawnPosition(
                tiles,
                tileSize,
                gridWidth,
                gridHeight,
                layer,
                MAX_ATTEMPTS,
                out Vector2 pos))
                continue;

            Tile tile = SpawnTile(grid, tilePrefab, data, pos);
            if (tile == null) continue;

            tile.layer = layer;
            tile.isBlocked = true;
            tile.UpdateSortingOrder();

            result.Add(tile);
            tiles.Add(tile);
        }

        return result;
    }


    // ================= SPAWN =================
    private static Tile SpawnTile(
        Transform grid,
        GameObject prefab,
        TileDataSO data,
        Vector2 pos
         )
    {
        GameObject go = Object.Instantiate(prefab, grid);
        go.transform.position = pos;

        Tile tile = go.GetComponent<Tile>();
        if (tile == null) return null;

        tile.tileData = data;
        tile.isBlocked = false;
        tile.isClicked = false;
        tile.layer = 0;

        tile.worldPos = pos;
        tile.ApplyData();

        return tile;
    }

    // ================= COLLISION =================
    private static bool IsOverlapping(
        Vector2 pos,
        Vector2 size,
        List<Tile> existing,
        List<Tile> created)
    {
        foreach (var t in existing.Concat(created))
        {
            if (t == null) continue;

            Vector2 dist = (Vector2)t.worldPos - pos;

            if (Mathf.Abs(dist.x) < size.x &&
                Mathf.Abs(dist.y) < size.y)
                return true;
        }
        return false;
    }
    private static int GetRandomLayer(List<Tile> tiles)
    {
        if (tiles.Count == 0) return 0;

        int maxLayer = tiles.Max(t => t.layer);
        return UnityEngine.Random.Range(0, maxLayer + 2);
    }
    private static bool TryFindSpawnPosition(
        List<Tile> tiles,
        Vector2 tileSize,
        float gridWidth,
        float gridHeight,
        int layer,
        int maxAttempts,
        out Vector2 position)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = UnityEngine.Random.Range(
                -gridWidth * 0.5f + tileSize.x * 0.5f,
                 gridWidth * 0.5f - tileSize.x * 0.5f);

            float y = UnityEngine.Random.Range(
                -gridHeight * 0.5f + tileSize.y * 0.5f,
                 gridHeight * 0.5f - tileSize.y * 0.5f);

            Vector2 pos = new(x, y);

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
        Vector2 pos,
        Vector2 size,
        int layer,
        List<Tile> tiles)
    {
        Rect newRect = new(pos - size * 0.5f, size);

        foreach (Tile tile in tiles)
        {
            Rect tileRect = new(
                (Vector2)tile.transform.position - size * 0.5f,
                size
            );

            if (newRect.Overlaps(tileRect) && tile.layer == layer)
                return true;
        }

        return false;
    }

}
