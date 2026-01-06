using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class LevelDataManager
{
    private static string editorFolderPath = "Assets/Resources/Levels";
    private const string runtimeResourcePath = "Levels";

    // =======================
    // EDITOR
    // =======================

    public static int GetTotalLevelEditor()
    {
#if UNITY_EDITOR
        if (!Directory.Exists(editorFolderPath))
            return 0;

        string[] files = Directory.GetFiles(editorFolderPath, "level_*.asset");
        if (files.Length == 0) return 0;

        return files
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Select(name =>
            {
                string[] parts = name.Split('_');
                return parts.Length > 1 && int.TryParse(parts[1], out int n) ? n : 0;
            })
            .Max();
#else
        return 0;
#endif
    }

    public static bool SaveToSO(Transform grid, int levelIndex)
    {
#if UNITY_EDITOR
        if (grid.GetComponentInChildren<Tile>() == null) return false;

        LevelDataSO asset = ScriptableObject.CreateInstance<LevelDataSO>();
        asset.tiles = new List<TileSaveData>();

        foreach (Tile tile in grid.GetComponentsInChildren<Tile>())
        {
            if (tile == null) continue;

            Transform shadowTf = tile.transform.Find("Container/Shadow");
            bool shadowActive = shadowTf != null && shadowTf.gameObject.activeSelf;

            asset.tiles.Add(new TileSaveData
            {
                tile = tile.tileData,
                worldPos = tile.transform.position,
                layer = tile.layer,
                isBlocked = tile.isBlocked,
                clicked = tile.isClicked,
                shadow = shadowActive
            });
        }

        if (!Directory.Exists(editorFolderPath))
            Directory.CreateDirectory(editorFolderPath);

        string path = $"{editorFolderPath}/level_{levelIndex}.asset";

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Saved Level (Editor): {path}");
        return true;
#else
        return false;
#endif
    }

    public static List<Tile> LoadFromSOEditor(
        int levelIndex,
        GameObject tilePrefab,
        Transform gridParent
    )
    {
#if UNITY_EDITOR
        string path = $"{editorFolderPath}/level_{levelIndex}.asset";
        LevelDataSO data = AssetDatabase.LoadAssetAtPath<LevelDataSO>(path);

        if (data == null)
        {
            Debug.LogError($"❌ Editor: Không tìm thấy {path}");
            return null;
        }

        ClearGridImmediate(gridParent);
        return SpawnTiles(data, tilePrefab, gridParent);
#else
        return null;
#endif
    }

    // =======================
    // RUNTIME (BUILD)
    // =======================

    public static List<Tile> LoadFromSORuntime(
        int levelIndex,
        GameObject tilePrefab,
        Transform gridParent
    )
    {
        string assetName = $"level_{levelIndex}";
        LevelDataSO data =
            Resources.Load<LevelDataSO>($"{runtimeResourcePath}/{assetName}");

        if (data == null)
        {
            Debug.LogError($"❌ Runtime: Không tìm thấy level {assetName}");
            return null;
        }

        ClearGrid(gridParent);
        return SpawnTiles(data, tilePrefab, gridParent);
    }

    public static int GetTotalLevelRuntime()
    {
        LevelDataSO[] levels =
            Resources.LoadAll<LevelDataSO>(runtimeResourcePath);
        return levels.Length;
    }

    // =======================
    // SHARED
    // =======================

    private static List<Tile> SpawnTiles(
        LevelDataSO data,
        GameObject tilePrefab,
        Transform gridParent
    )
    {
        List<Tile> tiles = new();

        foreach (var saveData in data.tiles)
        {
            GameObject tileObj =
                Object.Instantiate(tilePrefab, gridParent);

            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null) continue;

            tile.tileData = saveData.tile;
            tile.layer = saveData.layer;
            tile.isBlocked = saveData.isBlocked;
            tile.isClicked = saveData.clicked;
            tile.transform.position = saveData.worldPos;

            var food = tile.transform.Find("Container/Food");
            if (food != null)
                food.GetComponent<SpriteRenderer>().sprite =
                    saveData.tile.sprite;

            var shadow = tile.transform.Find("Container/Shadow");
            if (shadow != null)
                shadow.gameObject.SetActive(saveData.shadow);
            tile.UpdateSortingOrder();
            tiles.Add(tile);
            
        }

        return tiles;
    }

    private static void ClearGrid(Transform grid)
    {
        foreach (Transform child in grid)
            Object.Destroy(child.gameObject);
    }

#if UNITY_EDITOR
    private static void ClearGridImmediate(Transform grid)
    {
        foreach (Transform child in grid)
            Object.DestroyImmediate(child.gameObject);
    }
#endif
}
