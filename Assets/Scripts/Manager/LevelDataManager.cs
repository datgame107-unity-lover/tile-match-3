using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class LevelDataManager
{
    private static string folderPath = "Assets/Levels";

    public static bool SaveToSO(Transform grid, int levelIndex)
    {   
        if(grid.GetComponentInChildren<Tile>() ==null) return false;
#if UNITY_EDITOR
        // Tạo asset mới
        LevelDataSO asset = ScriptableObject.CreateInstance<LevelDataSO>();
        asset.tiles = new List<TileSaveData>();

        foreach (Tile tile in grid.GetComponentsInChildren<Tile>())
        {
            if (tile == null) continue;

            asset.tiles.Add(new TileSaveData
            {
                tile = tile.tileData,
                worldPos = tile.transform.position,
                gridPos = tile.gridPos,
                layer = tile.layer,
                isBlocked = tile.isBlocked,
                clicked = tile.isClicked
            });
        }

        // Tạo folder nếu chưa có
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = $"{folderPath}/level_{levelIndex}.asset";

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Saved Level to: {path}");
        return true;
#else
        Debug.LogWarning("⚠ SaveToSO chỉ hoạt động trong Editor");
#endif
    }

    public static List<Tile> LoadFromSO(int levelIndex, GameObject tilePrefab, Transform gridParent)
    {
#if UNITY_EDITOR
        string filePath = $"{folderPath}/level_{levelIndex}.asset";

        // Load SO
        LevelDataSO data = AssetDatabase.LoadAssetAtPath<LevelDataSO>(filePath);

        if (data == null)
        {
            Debug.LogError($"❌ Không tìm thấy file: {filePath}");
            return null;
        }

        // Clear tile cũ
        foreach (Transform child in gridParent)
            Object.DestroyImmediate(child.gameObject);

        List<Tile> loadedTiles = new List<Tile>();

        foreach (var saveData in data.tiles)
        {
            // Instantiate tile
            GameObject tileObj = PrefabUtility.InstantiatePrefab(tilePrefab, gridParent) as GameObject;
            Tile tile = tileObj.GetComponent<Tile>();

            if (tile == null)
            {
                Debug.LogError("❌ Prefab không có component Tile");
                continue;
            }

            // Gán dữ liệu
            tile.tileData = saveData.tile;
            tile.gridPos = saveData.gridPos;
            tile.layer = saveData.layer;
            tile.isBlocked = saveData.isBlocked;
            tile.isClicked = saveData.clicked;

            // Set vị trí thực
            tile.transform.position = saveData.worldPos;

            // Set sprites từ tileData

            tile.transform.Find("Container/Food").GetComponent<SpriteRenderer>().sprite =
                saveData.tile.sprite;

            loadedTiles.Add(tile);
        }

        Debug.Log($"✅ Loaded Level_{levelIndex}.asset thành công!");

        return loadedTiles;
#else
        Debug.LogWarning("⚠ LoadFromSO chỉ hoạt động trong Editor!");
        return null;
#endif
    }
}
