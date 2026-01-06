using System.Linq;
using Unity.Jobs;
using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public static LevelEditorManager Instance;

    [Header("Grid Config")]
    public Transform grid;                 // grid root (TÂM grid)
    public int baseWidth = 5;
    public int baseHeight = 7;
    public float cellSize = 0.8f;
    public bool allowHalfCell = true;

    [Header("Shadow")]
    public GameObject shadow;

    [Header("Tile Placement")]
    public GameObject tilePrefab;
    public TileDataSO selectedTile;

    [Header("Runtime")]
    public bool isDragging;
    public bool isChanged;
    public int currentLevel;

    private Camera cam;

    #region ===== UNITY =====

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }


    private void OnEnable()
    {
        EventManager.OnSavingNewLevel += HandleSavingLevel;
    }

    private void OnDisable()
    {
        EventManager.OnSavingNewLevel -= HandleSavingLevel;
    }

    private void HandleSavingLevel()
    {
        if(currentLevel == 0)
        {
            LevelDataManager.SaveToSO(grid, LevelDataManager.GetTotalLevelEditor()+1);
            
        }
        else
        {
            LevelDataManager.SaveToSO(grid,currentLevel);

        }

        EventManager.OnSavedNewLevel?.Invoke();
    }
    private void Update()
    {
        UpdateShadowSnap();
    }

    #endregion

    #region ===== GRID CALC =====

    float Step => allowHalfCell ? cellSize / 2f : cellSize;
    int GridWidth => allowHalfCell ? baseWidth * 2 : baseWidth;
    int GridHeight => allowHalfCell ? baseHeight * 2 : baseHeight;

    Vector2 GridSize =>
        new Vector2(GridWidth * Step, GridHeight * Step);

    Vector3 BottomLeft =>
        grid.position - new Vector3(GridSize.x / 2f, GridSize.y / 2f, 0f);

    Vector2Int WorldToGrid(Vector3 world)
    {
        Vector3 local = world - BottomLeft;
        int x = Mathf.FloorToInt(local.x / Step);
        int y = Mathf.FloorToInt(local.y / Step);
        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return BottomLeft +
               new Vector3(
                   gridPos.x * Step + Step * 0.5f,
                   gridPos.y * Step + Step * 0.5f,
                   0f
               );
    }

    bool IsInsideGrid(Vector2Int p)
    {
        return p.x >= 0 && p.x < GridWidth &&
               p.y >= 0 && p.y < GridHeight;
    }

    #endregion

    #region ===== SHADOW =====

    void UpdateShadowSnap()
    {
        if (!isDragging || selectedTile == null || shadow == null)
        {
            shadow.SetActive(false);
            return;
        }

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0;

        Vector2Int gp = WorldToGrid(world);

        if (!IsInsideGrid(gp))
        {
            shadow.SetActive(false);
            return;
        }

        shadow.SetActive(true);
        shadow.transform.position = GridToWorld(gp);
    }

    #endregion

    #region ===== TILE PLACE =====

    public void TryPlaceSelectedTile(Vector2 screenPos)
    {
        if (selectedTile == null)
            return;

        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0;

        Vector2Int gp = WorldToGrid(world);
        if (!IsInsideGrid(gp))
            return;

        Vector3 spawnPos = GridToWorld(gp);

        int highestLayer = GetHighestLayerAt(spawnPos);
        int newLayer = highestLayer + 1;

        GameObject go = Instantiate(tilePrefab, spawnPos, Quaternion.identity, grid);

        Tile t = go.GetComponent<Tile>();
        t.tileData = selectedTile;
        t.layer = newLayer;
        t.UpdateSortingOrder();

        var food = go.transform.Find("Container/Food")?.GetComponent<SpriteRenderer>();
        if (food != null)
            food.sprite = selectedTile.sprite;

        isChanged = true;
        Sort();
    }

    int GetHighestLayerAt(Vector3 worldPos)
    {
        int highest = -1;

        BoxCollider2D col =
            tilePrefab.GetComponentInChildren<BoxCollider2D>();

        if (col == null)
            return highest;

        Vector2 size = Vector2.Scale(col.size, col.transform.lossyScale);
        Vector2 center = (Vector2)worldPos + col.offset;

        var hits = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (var h in hits)
        {
            Tile t = h.GetComponent<Tile>();
            if (t != null && t.layer > highest)
                highest = t.layer;
        }

        return highest;
    }

    #endregion

    #region ===== SORT =====

    public   void Sort()
    {
        var tiles = grid.GetComponentsInChildren<Tile>().ToList();
        SortTiles.Sort(tiles);
        SortTiles.ActivateShadows(tiles);
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        // khung
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(grid.position,
            new Vector3(GridSize.x, GridSize.y, 0));

        // ô
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                Vector3 c = GridToWorld(new Vector2Int(x, y));
                Gizmos.DrawWireCube(c, Vector3.one * Step);
            }
        }

        // (0,0)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GridToWorld(Vector2Int.zero), Step * 0.2f);
    }
#endif
}
