using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class LevelDataManager
{
    private static string folderPath = "Assets/Levels";

    public static int GetTotalLevel()
    {
        int totalLevel = 0;
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("📁 Thư mục Assets/Levels chưa tồn tại!");
            return 0;
        }

        string[] files = Directory.GetFiles(folderPath, "Level_*.asset");

        if (files.Length == 0)
        {
            Debug.Log("⚠️ Không có level nào được lưu!");
        }
        else
        {
            var levelNumbers = files
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Select(name =>
            {
                string[] parts = name.Split('_');
                if (parts.Length > 1 && int.TryParse(parts[1], out int n))
                    return n;
                return 0;
            })
            .ToList();

             totalLevel = levelNumbers.Max();
        }
            return totalLevel;

    }
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

    public static void LoadFromSO(int levelIndex, GameObject tilePrefab, Transform gridParent)
    {
#if UNITY_EDITOR
        string filePath = $"{folderPath}/level_{levelIndex}.asset";

        // Load SO
        LevelDataSO data = AssetDatabase.LoadAssetAtPath<LevelDataSO>(filePath);

        if (data == null)
        {
            Debug.LogError($"❌ Không tìm thấy file: {filePath}");
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

#else
        Debug.LogWarning("⚠ LoadFromSO chỉ hoạt động trong Editor!");
        return null;
#endif
    }
}
