// Scripts/LevelCreate/BoardController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GridMode { Full, Half }

public class BoardController : MonoBehaviour
{
    [Header("Config")]
    public GridConfig config;
    public GridMode gridMode = GridMode.Full;

    [Header("Refs")]
    public GameObject tilePrefab;
    public TileDataSO[] allTileData;

    private readonly Dictionary<string, Tile> _placed = new Dictionary<string, Tile>();

    private float Step => gridMode == GridMode.Full ? config.cellSize : config.cellSize * 0.5f;

    // ── Key ──────────────────────────────────────────────
    private string Key(Vector2 p, int layer)
    {
        int x = Mathf.RoundToInt(p.x * 100);
        int y = Mathf.RoundToInt(p.y * 100);
        return $"{x}_{y}_{layer}";
    }

    // ── Snap ─────────────────────────────────────────────
    public Vector2 Snap(Vector3 worldPos)
    {
        float s = Step;
        float x = Mathf.Round(worldPos.x / s) * s;
        float y = Mathf.Round(worldPos.y / s) * s;
        x = Mathf.Round(x * 100f) / 100f;
        y = Mathf.Round(y * 100f) / 100f;
        return new Vector2(x, y);
    }

    // ── Update — click phải xóa tile ─────────────────────
    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var mouseScreen = Mouse.current.position.ReadValue();
            var world = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseScreen.x, mouseScreen.y,
                    -Camera.main.transform.position.z));
            world.z = 0;
            bool removed = TryRemove(world);
            if (removed) FindFirstObjectByType<LevelPanelUI>()?.MarkDirty();
        }
    }

    // ── Place ────────────────────────────────────────────
    public bool TryPlace(TileDataSO data, Vector3 worldPos)
    {
        var snapped = Snap(worldPos);
        int layer = CalcLayer(snapped);
        var key = Key(snapped, layer);

        if (_placed.ContainsKey(key)) return false;

        var go = Instantiate(tilePrefab,
            new Vector3(snapped.x, snapped.y, -layer * 0.1f),
            Quaternion.identity, transform);

        var tile = go.GetComponent<Tile>();
        tile.Init(data, layer);
        _placed[key] = tile;

        RefreshBlockingAround(snapped);
        return true;
    }

    // ── Remove — xóa tile layer cao nhất tại vị trí ─────
    public bool TryRemove(Vector3 worldPos)
    {
        var snapped = Snap(worldPos);

        Tile topTile = null;
        string topKey = null;
        int topLayer = -1;

        foreach (var kv in _placed)
        {
            var t = kv.Value;
            if (IsOverlapping(snapped, t.transform.position) && t.layer > topLayer)
            {
                topTile = t;
                topKey = kv.Key;
                topLayer = t.layer;
            }
        }

        if (topTile == null) return false;

        Destroy(topTile.gameObject);
        _placed.Remove(topKey);
        RefreshBlockingAround(snapped);
        return true;
    }

    // ── Export ───────────────────────────────────────────
    public LevelSaveData Export(int levelIndex)
    {
        var data = new LevelSaveData { levelIndex = levelIndex };
        foreach (var kv in _placed)
        {
            var t = kv.Value;
            data.tiles.Add(new TilePlacement
            {
                tileId = t.tileData.tileId,
                x = t.transform.position.x,
                y = t.transform.position.y,
                layer = t.layer
            });
        }
        return data;
    }

    // ── Load ─────────────────────────────────────────────
    public void LoadIntoBoard(LevelSaveData data)
    {
        ClearBoard();
        foreach (var p in data.tiles)
        {
            var td = FindTileData(p.tileId);
            if (td == null) continue;

            var go = Instantiate(tilePrefab,
                new Vector3(p.x, p.y, -p.layer * 0.1f),
                Quaternion.identity, transform);

            var tile = go.GetComponent<Tile>();
            tile.Init(td, p.layer);
            _placed[Key(new Vector2(p.x, p.y), p.layer)] = tile;
        }
        RefreshAllBlocking();
    }

    public void ClearBoard()
    {
        foreach (var kv in _placed)
            if (kv.Value != null) Destroy(kv.Value.gameObject);
        _placed.Clear();
    }

    // ── Layer ────────────────────────────────────────────
    private int CalcLayer(Vector2 pos)
    {
        int max = 0;
        foreach (var kv in _placed)
        {
            var other = kv.Value;
            if (IsOverlapping(pos, other.transform.position))
                max = Mathf.Max(max, other.layer + 1);
        }
        return max;
    }

    private bool IsOverlapping(Vector2 a, Vector2 b) =>
        Mathf.Abs(a.x - b.x) < config.colliderSize &&
        Mathf.Abs(a.y - b.y) < config.colliderSize;

    // ── Blocking ─────────────────────────────────────────
    private void RefreshBlockingAround(Vector2 center)
    {
        foreach (var kv in _placed)
            if (IsOverlapping(center, kv.Value.transform.position))
                RefreshBlocking(kv.Value);
    }

    private void RefreshAllBlocking()
    {
        foreach (var kv in _placed) RefreshBlocking(kv.Value);
    }

    private void RefreshBlocking(Tile tile)
    {
        bool blocked = false;
        foreach (var kv in _placed)
        {
            var other = kv.Value;
            if (other == tile) continue;
            if (other.layer > tile.layer &&
                IsOverlapping(tile.transform.position, other.transform.position))
            {
                blocked = true;
                break;
            }
        }
        tile.SetBlocked(blocked);
    }

    // ── Gizmos ───────────────────────────────────────────
    private void OnDrawGizmos()
    {
        float s = Step;
        float w = config.columns * config.cellSize;
        float h = config.rows * config.cellSize;
        float ox = -w / 2f;
        float oy = -h / 2f;

        Gizmos.color = new Color(0.6f, 0.6f, 1f, 0.25f);
        int vLines = Mathf.RoundToInt(w / s) + 1;
        for (int i = 0; i < vLines; i++)
            Gizmos.DrawLine(new Vector3(ox + i * s, oy), new Vector3(ox + i * s, oy + h));

        int hLines = Mathf.RoundToInt(h / s) + 1;
        for (int j = 0; j < hLines; j++)
            Gizmos.DrawLine(new Vector3(ox, oy + j * s), new Vector3(ox + w, oy + j * s));

        Gizmos.color = new Color(0.4f, 0.4f, 1f, 0.6f);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(w, h, 0));

#if UNITY_EDITOR
        if (Application.isPlaying && Camera.main != null)
        {
            var ms = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            var world = Camera.main.ScreenToWorldPoint(
                new Vector3(ms.x, ms.y, -Camera.main.transform.position.z));
            var snapped = Snap(world);
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireCube(new Vector3(snapped.x, snapped.y, 0),
                Vector3.one * s * 0.95f);
        }
#endif
    }

    // ── Helper ───────────────────────────────────────────
    private TileDataSO FindTileData(string id)
    {
        foreach (var t in allTileData)
            if (t.name == id) return t;
        Debug.LogWarning($"[Board] TileData not found: {id}");
        return null;
    }
}