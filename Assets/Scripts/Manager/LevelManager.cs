using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

public static class LevelManager
{
    public static List<Tile> GenerateTiles(Transform grid, List<TileDataSO> tileDatas, int spawnCount)
    {
        var tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TilePrefab.prefab");
        if (tilePrefab == null)
        {
            Debug.LogError("Không tìm thấy TilePrefab!");
            return new List<Tile>();
        }

        List<Tile> createdTiles = new List<Tile>();
        int maxAttempts = 50;
        float gridWidth = 4f;
        float gridHeight = 6f;
        float padding = 0.1f; // khoảng cách tối thiểu giữa các tile

        BoxCollider2D prefabBox = tilePrefab.GetComponent<BoxCollider2D>();
        Vector2 tileSize = prefabBox != null ? prefabBox.size + new Vector2(padding, padding) : new Vector2(1f, 1f);

        for (int i = 0; i < spawnCount; i++)
        {
            TileDataSO data = tileDatas[Random.Range(0, tileDatas.Count-4)];
            Vector2 spawnPos = Vector2.zero;
            bool positionFound = false;

            // Tìm vị trí hợp lệ
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                float xPos = Random.Range(-gridWidth / 2f + tileSize.x / 2f, gridWidth / 2f - tileSize.x / 2f);
                float yPos = Random.Range(-gridHeight / 2f + tileSize.y / 2f, gridHeight / 2f - tileSize.y / 2f);
                spawnPos = new Vector2(xPos, yPos);

                // Check overlap với các tile đã spawn
                bool overlap = false;
                foreach (var t in createdTiles)
                {
                    Vector2 dist = (Vector2)t.worldPos -spawnPos;
                    if (Mathf.Abs(dist.x) < tileSize.x && Mathf.Abs(dist.y) < tileSize.y)
                    {
                        overlap = true;
                        break;
                    }
                }

                if (!overlap)
                {
                    positionFound = true;
                    break;
                }
            }

            if (!positionFound)
            {
                continue;
            }

            // Instantiate tile sau khi đã chọn vị trí hợp lệ
            GameObject tileGO = GameObject.Instantiate(tilePrefab, grid);
            tileGO.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);

            Tile tile = tileGO.GetComponent<Tile>();
            if (tile == null) continue;

            tile.tileData = data;
            tile.isBlocked = false;
            tile.isClicked = false;
            tile.layer = 0;
            tile.transform.Find("Container/Food").GetComponent<SpriteRenderer>().sprite = data.sprite;
            tile.worldPos = tile.transform.position;

            createdTiles.Add(tile);
        }

        return createdTiles;
    }




}
