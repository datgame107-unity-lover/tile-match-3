using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;

public static class TileSpawner
{
    public static Tile Spawn(
        Transform parent,
        GameObject prefab,
        TileDataSO data,
        Vector2 pos,
        int layer)
    {
        var go = Object.Instantiate(prefab, parent);
        go.transform.position = pos;

        var tile = go.GetComponent<Tile>();
        tile.Init(data, layer);

        return tile;
    }
    public static List<Tile> SpawnTiles(
    Transform parent,
    GameObject prefab,
    LevelRuntimeData levelData)   // ← đổi sang RuntimeData
    {
        List<Tile> result = new();
        foreach (var entry in levelData.tiles)
        {
            var go = Object.Instantiate(prefab, parent);
            go.transform.position = new Vector3(entry.worldPos.x,entry.worldPos.y, -entry.layer * 0.1f);
            var tile = go.GetComponent<Tile>();
            tile.Init(entry.tileData, entry.layer);  // ← dùng TileDataSO, không phải string
            result.Add(tile);
        }
        return result;
    }

}