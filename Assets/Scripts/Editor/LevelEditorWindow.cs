using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelDataSO levelData;
    private TileDataSO selectedTile;

    [Header("Grid")]
    [SerializeField] private Transform gridOrigin;
    private bool allowHalfCell = false;
    private const float cellSize = 0.8f;

    private readonly Color validShadowColor = new(1f, 1f, 1f, 0.35f);
    private readonly Color invalidShadowColor = new(1f, 0f, 0f, 0.35f);

    [MenuItem("Tools/Level Editor")]
    static void Open()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    #region GUI

    private void OnGUI()
    {
        levelData = (LevelDataSO)EditorGUILayout.ObjectField(
            "Level Data", levelData, typeof(LevelDataSO), false);

        selectedTile = (TileDataSO)EditorGUILayout.ObjectField(
            "Tile", selectedTile, typeof(TileDataSO), false);

        gridOrigin = (Transform)EditorGUILayout.ObjectField(
            "Grid Origin", gridOrigin, typeof(Transform), true);

        allowHalfCell = EditorGUILayout.Toggle("Allow Half Cell", allowHalfCell);

        if (levelData == null || selectedTile == null)
        {
            EditorGUILayout.HelpBox("Assign LevelData & Tile", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Clear Level"))
        {
            if (EditorUtility.DisplayDialog(
                "Clear Level",
                "Remove all tiles?",
                "Yes",
                "Cancel"))
            {
                Undo.RecordObject(levelData, "Clear Level");
                levelData.tiles.Clear();
                EditorUtility.SetDirty(levelData);
            }
        }
    }

    #endregion

    #region Scene GUI

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView view)
    {
        if (levelData == null || selectedTile == null)
            return;

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new(Vector3.forward, Vector3.zero);

        if (!plane.Raycast(ray, out float dist))
            return;

        Vector3 worldPos = ray.GetPoint(dist);
        Vector2Int gridPos = WorldToGrid(worldPos);

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            PlaceTile(gridPos);
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            RemoveTile(gridPos);
            e.Use();
        }

        DrawGrid();
        DrawPlacedTiles();
        DrawShadow(gridPos);
        DrawPreview(gridPos);

        view.Repaint();
    }

    #endregion

    #region Grid Logic

    Vector3 GridOriginPos =>
        gridOrigin != null ? gridOrigin.position : Vector3.zero;

    Vector3 GridToWorld(Vector2Int pos)
    {
        return GridOriginPos +
               new Vector3(
                   (pos.x + 0.5f) * cellSize,
                   (pos.y + 0.5f) * cellSize,
                   0);
    }

    Vector2Int WorldToGrid(Vector3 world)
    {
        world -= GridOriginPos;

        float step = allowHalfCell ? cellSize / 2f : cellSize;
        int x = Mathf.FloorToInt(world.x / step);
        int y = Mathf.FloorToInt(world.y / step);
        return new Vector2Int(x, y);
    }

    bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < levelData.width &&
               pos.y >= 0 && pos.y < levelData.height;
    }

    bool HasTileAt(Vector2Int pos)
    {
        return levelData.tiles.Any(t => t.gridPos == pos);
    }

    #endregion

    #region Draw

    void DrawGrid()
    {
        Handles.color = Color.gray;

        Vector3 o = GridOriginPos;

        for (int x = 0; x <= levelData.width; x++)
        {
            Handles.DrawLine(
                o + new Vector3(x * cellSize, 0),
                o + new Vector3(x * cellSize, levelData.height * cellSize));
        }

        for (int y = 0; y <= levelData.height; y++)
        {
            Handles.DrawLine(
                o + new Vector3(0, y * cellSize),
                o + new Vector3(levelData.width * cellSize, y * cellSize));
        }
    }

    void DrawPreview(Vector2Int gridPos)
    {
        bool inside = IsInsideGrid(gridPos);
        bool occupied = HasTileAt(gridPos);

        Color fill = (!inside || occupied)
            ? new Color(1f, 0f, 0f, 0.25f)
            : new Color(0f, 1f, 0f, 0.2f);

        Vector3 center = GridToWorld(gridPos);

        Handles.DrawSolidRectangleWithOutline(
            new Rect(center - Vector3.one * cellSize / 2f,
                     Vector2.one * cellSize),
            fill,
            Color.black);
    }

    void DrawPlacedTiles()
    {
        Handles.BeginGUI();

        foreach (var t in levelData.tiles)
        {
            if (t.tile == null || t.tile.sprite == null)
                continue;

            Vector3 world = GridToWorld(t.gridPos);
            Rect rect = WorldToGUIRect(world, cellSize);

            DrawSprite(rect, t.tile.sprite);
        }

        Handles.EndGUI();
    }

    void DrawShadow(Vector2Int gridPos)
    {
        if (selectedTile.sprite == null)
            return;

        bool valid = IsInsideGrid(gridPos) && !HasTileAt(gridPos);
        Color shadowColor = valid ? validShadowColor : invalidShadowColor;

        Vector3 world = GridToWorld(gridPos) +
                        new Vector3(0.04f, -0.04f, 0);

        Rect rect = WorldToGUIRect(world, cellSize);

        Handles.BeginGUI();
        Color old = GUI.color;
        GUI.color = shadowColor;

        DrawSprite(rect, selectedTile.sprite);

        GUI.color = old;
        Handles.EndGUI();
    }

    Rect WorldToGUIRect(Vector3 world, float size)
    {
        SceneView view = SceneView.lastActiveSceneView;
        Camera cam = view.camera;

        Vector2 guiPos = HandleUtility.WorldToGUIPoint(world);

        float pixelSize = size * cam.pixelHeight /
                          (cam.orthographicSize * 2f);

        return new Rect(
            guiPos.x - pixelSize / 2f,
            guiPos.y - pixelSize / 2f,
            pixelSize,
            pixelSize);
    }

    void DrawSprite(Rect rect, Sprite sprite)
    {
        Rect uv = new Rect(
            sprite.rect.x / sprite.texture.width,
            sprite.rect.y / sprite.texture.height,
            sprite.rect.width / sprite.texture.width,
            sprite.rect.height / sprite.texture.height
        );

        GUI.DrawTextureWithTexCoords(rect, sprite.texture, uv);
    }

    #endregion

    #region Place / Remove

    void PlaceTile(Vector2Int pos)
    {
        if (!IsInsideGrid(pos))
            return;

        int nextLayer = levelData.tiles
            .Where(t => t.gridPos == pos)
            .Select(t => t.layer)
            .DefaultIfEmpty(-1)
            .Max() + 1;

        Undo.RecordObject(levelData, "Place Tile");

        levelData.tiles.Add(new TileSaveData
        {
            gridPos = pos,
            tile = selectedTile,
            layer = nextLayer
        });

        EditorUtility.SetDirty(levelData);
    }

    void RemoveTile(Vector2Int pos)
    {
        if (!HasTileAt(pos))
            return;

        Undo.RecordObject(levelData, "Remove Tile");
        levelData.tiles.RemoveAll(t => t.gridPos == pos);
        EditorUtility.SetDirty(levelData);
    }

    #endregion
}
