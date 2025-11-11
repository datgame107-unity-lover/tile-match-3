using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{   
    public static LevelManager Instance;



    [Header("Shape Config")]
    public BaseShapeSO shape;

    [Header("Tile Settings")]
    public GameObject tilePrefab;
    public bool useSortingOrder = true;

    [Header("Clear Before Generate")]
    public bool autoClearOldTiles = true;

    [Header("Overlap Resolve")]
    public int resolveIterations = 8;
    public float minDistanceMultiplier = 0.9f;
    public bool useRendererBoundsForSpacing = true;

    public TMP_InputField levelInput;
    public TileDataSO[] tileDatas;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
#if UNITY_EDITOR
        string folderPath = "Assets/Levels";
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("📁 Thư mục Assets/Levels chưa tồn tại!");
            levelInput.text = "1";
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "Level_*.asset");

        if (files.Length == 0)
        {
            Debug.Log("⚠️ Không có level nào được lưu!");
            levelInput.text = "0";
            return;
        }

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

        int maxLevel = levelNumbers.Max();
        if(levelInput!=null)    
        levelInput.text = maxLevel.ToString();
        Debug.Log($"📘 Level cao nhất: {maxLevel}");
#else
        levelInput.text = "0";
#endif
    }
    public void Generate()
    {
        if (shape == null || tilePrefab == null)
        {
            Debug.LogWarning("⚠️ Missing Shape or Tile Prefab!");
            return;
        }

        if (tileDatas == null || tileDatas.Length == 0)
        {
            Debug.LogWarning("⚠️ Chưa gán TileDataSO trong LevelManager!");
            return;
        }

        if (autoClearOldTiles)
            ClearGeneratedTiles();

        Vector2[] basePositions = shape.GetTilePositions();
        int totalTiles = basePositions.Length * shape.layerCount;

        int validTotal = totalTiles - (totalTiles % 3); // đảm bảo chia hết cho 3
        int remainder = totalTiles % 3;

        if (remainder > 0)
            Debug.Log($"⚠️ Tổng số tile ({totalTiles}) không chia hết cho 3, bỏ {remainder} tile cuối.");

        totalTiles = validTotal;

        List<TileDataSO> randomDataList = new List<TileDataSO>();

        int perType = Mathf.Max(3, (totalTiles / tileDatas.Length) / 3 * 3); // mỗi loại chia hết cho 3
        foreach (var data in tileDatas)
        {
            for (int i = 0; i < perType; i++)
                randomDataList.Add(data);
        }

        while (randomDataList.Count < totalTiles)
            randomDataList.Add(tileDatas[Random.Range(0, tileDatas.Length)]);

        for (int i = 0; i < randomDataList.Count; i++)
        {
            int rand = Random.Range(i, randomDataList.Count);
            (randomDataList[i], randomDataList[rand]) = (randomDataList[rand], randomDataList[i]);
        }

        int index = 0;

        for (int layer = 0; layer < shape.layerCount; layer++)
        {
            List<Tile> createdThisLayer = new List<Tile>();

            foreach (Vector2 basePos in basePositions)
            {
                if (index >= totalTiles) break;

                Vector2 spawnPos = basePos;
                if (!shape.allowOverlap)
                    spawnPos += shape.layerOffset * layer;

                Tile tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform).GetComponent<Tile>();
                tile.layer = layer;
                tile.worldPos = tile.transform.position;
                tile.gridPos = Vector2Int.RoundToInt(basePos);
                tile.isBlocked = layer != shape.layerCount - 1;

                tile.tileData = randomDataList[index];  
                tile.transform.Find("Food").GetComponent<SpriteRenderer>().sprite = tile.tileData.sprite;
                createdThisLayer.Add(tile);
                index++;
            }

            ResolveOverlapUsingCircleCollider(createdThisLayer);
        }

        Debug.Log($"✅ Đã tạo {totalTiles} tile (chia hết cho 3), mỗi tileData có khoảng {perType} tile.");
    }


    public void ClearGeneratedTiles()
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
            children.Add(child);
        foreach (Transform t in children)
            DestroyImmediate(t.gameObject);
    }
   
    private void ResolveOverlapUsingCircleCollider(List<Tile> tiles)
    {
        for (int iter = 0; iter < resolveIterations; iter++)
        {
            bool movedAny = false;

            for (int i = 0; i < tiles.Count; i++)
            {
                for (int j = i + 1; j < tiles.Count; j++)
                {
                    var col1 = tiles[i].GetComponent<CircleCollider2D>();
                    var col2 = tiles[j].GetComponent<CircleCollider2D>();
                    if (col1 == null || col2 == null)
                        continue;

                    Vector2 pos1 = col1.bounds.center;
                    Vector2 pos2 = col2.bounds.center;

                    float radiusSum = col1.bounds.extents.x + col2.bounds.extents.x;
                    float dist = Vector2.Distance(pos1, pos2);
                    float minDist = radiusSum * minDistanceMultiplier;

                    if (dist < minDist)
                    {
                        Vector2 direction = (pos2 - pos1).normalized;
                        if (direction == Vector2.zero)
                            direction = Vector2.right;

                        float overlap = (minDist - dist) / 2f;
                        col1.transform.position -= (Vector3)(direction * overlap);
                        col2.transform.position += (Vector3)(direction * overlap);
                        movedAny = true;
                    }
                }
            }

            if (!movedAny) break;
        }
    }

 
    public void SaveToSO()
    {
#if UNITY_EDITOR
        var asset = ScriptableObject.CreateInstance<LevelDataSO>();
        asset.tiles = new List<TileSaveData>();

        foreach (var tile in GetComponentsInChildren<Tile>())
        {
            if (tile == null) continue;

            asset.tiles.Add(new TileSaveData
            {
                tile = tile.tileData,
                worldPos = tile.worldPos,
                gridPos = tile.gridPos,
                layer = tile.layer,
                isBlocked = tile.isBlocked,
                clicked = tile.isClicked
            });
        }

        string folderPath = "Assets/Levels";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = $"{folderPath}/level_{levelInput.text}.asset";

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Đã lưu Level ScriptableObject tại: {path}");
#else
        Debug.LogWarning("⚠️ SaveToSO() chỉ hoạt động trong Unity Editor!");
#endif
    }
    public void LoadFromSO()
    {
#if UNITY_EDITOR
        ClearGeneratedTiles();

        string folderPath = "Assets/Levels";
        string fileName = $"level_{levelInput.text}.asset";
        string fullPath = Path.Combine(folderPath, fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"❌ Không tìm thấy file: {fullPath}");
            return;
        }

        LevelDataSO levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);
        if (levelData == null)
        {
            Debug.LogError("❌ Không thể load LevelDataSO!");
            return;
        }

        foreach (var tileSave in levelData.tiles)
        {
            if (tileSave == null) continue;

            GameObject tileObj = Instantiate(tilePrefab, tileSave.worldPos, Quaternion.identity, transform);
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null)
            {
                Debug.LogWarning("⚠️ Prefab không có component Tile!");
                continue;
            }

            tile.tileData = tileSave.tile;  
            tile.worldPos = tileSave.worldPos;
            tile.gridPos = tileSave.gridPos;
            tile.layer = tileSave.layer;
            tile.isBlocked = tileSave.isBlocked;
            tile.isClicked = tileSave.clicked;
            tile.transform.Find("Food").GetComponent<SpriteRenderer>().sprite = tileSave.tile.sprite;
        }

      
        Debug.Log($"✅ Load Level thành công từ {fileName}");
#else
    Debug.LogWarning("⚠️ LoadFromSO() chỉ hoạt động trong Unity Editor!");
#endif
    }
    public  void LoadFromSO(int level)
    {
#if UNITY_EDITOR
        ClearGeneratedTiles();

        string folderPath = "Assets/Levels";
        string fileName = $"level_{level}.asset";
        string fullPath = Path.Combine(folderPath, fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"❌ Không tìm thấy file: {fullPath}");
            return;
        }

        LevelDataSO levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(fullPath);
        if (levelData == null)
        {
            Debug.LogError("❌ Không thể load LevelDataSO!");
            return;
        }

        foreach (var tileSave in levelData.tiles)
        {
            if (tileSave == null) continue;

            GameObject tileObj = Instantiate(tilePrefab, tileSave.worldPos, Quaternion.identity, transform);
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null)
            {
                Debug.LogWarning("⚠️ Prefab không có component Tile!");
                continue;
            }

            tile.tileData = tileSave.tile;  // <-- gán trực tiếp TileDataSO
            tile.worldPos = tileSave.worldPos;
            tile.gridPos = tileSave.gridPos;
            tile.layer = tileSave.layer;
            tile.isBlocked = tileSave.isBlocked;
            tile.isClicked = tileSave.clicked;
            tile.transform.Find("Food").GetComponent<SpriteRenderer>().sprite = tileSave.tile.sprite;
        }


        Debug.Log($"✅ Load Level thành công từ {fileName}");
#else
    Debug.LogWarning("⚠️ LoadFromSO() chỉ hoạt động trong Unity Editor!");
#endif
    }

    void OnDrawGizmos()
    {
        foreach (Transform child in transform)
        {
            var col = child.GetComponent<CircleCollider2D>();
            if (col != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(col.bounds.center, col.bounds.extents.x);
            }
        }
    }
}
