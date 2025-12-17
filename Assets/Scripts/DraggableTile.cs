using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public GameObject tilePrefabToSpawn;
    public TileDataSO tileData;
    [Header("Grid")]
    [SerializeField] private Transform grid; 
    public float cellPadding = 0.05f;
    private GameObject dragContainer;
    [SerializeField] private bool allowHalfCell = false; // cho phép snap nửa cell

    private Vector3 debugSnapPos;
    private Vector2 debugBoxSize = new Vector2(0.75f, 0.76f);

    private void Awake()
    {
       
    }
    private void Start()
    {
        GameObject gridObject = GameObject.Find("Grid");
        if (gridObject != null)
            grid = gridObject.transform;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject tempTile = Instantiate(tilePrefabToSpawn);
        dragContainer = tempTile.transform.Find("Container").gameObject;

        dragContainer.transform.SetParent(null);
        Destroy(tempTile);
        var bg = dragContainer.transform.Find("Base").GetComponent<SpriteRenderer>();
        var food = dragContainer.transform.Find("Food").GetComponent<SpriteRenderer>();
        bg.sortingLayerName = "DraggableTile";
        food.sortingLayerName = "DraggableTile";
        food.sprite = tileData.sprite;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;
        dragContainer.transform.position = worldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragContainer == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;
        dragContainer.transform.position = worldPos;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragContainer == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;

        Vector2 cellSize = new Vector2(0.78f, 0.79f);
        float stepX = allowHalfCell ? cellSize.x / 2f : cellSize.x;
        float stepY = allowHalfCell ? cellSize.y / 2f : cellSize.y;

        float snapX = Mathf.Floor(worldPos.x / stepX + 0.5f) * stepX;
        float snapY = Mathf.Floor(worldPos.y / stepY + 0.5f) * stepY;
        Vector3 snapPos = new Vector3(snapX, snapY, 0f);

        // =======================
        // Kiểm tra nằm trong grid 4x6
        // Giả sử grid center tại (0,0)
        float gridWidth = 5f * cellSize.x;
        float gridHeight = 7f * cellSize.y;
        float minX = -gridWidth / 2f;
        float maxX = gridWidth / 2f;
        float minY = -gridHeight / 2f;
        float maxY = gridHeight / 2f;

        if (snapPos.x < minX || snapPos.x > maxX || snapPos.y < minY || snapPos.y > maxY)
        {
            Destroy(dragContainer);
            dragContainer = null;
            return; // thoát luôn, không instantiate tile
        }
        // =======================

        BoxCollider2D col = tilePrefabToSpawn.GetComponentInChildren<BoxCollider2D>();
        Vector2 realSize = Vector2.Scale(col.size * 0.85f, col.transform.lossyScale);
        Vector2 realOffset = Vector2.Scale(col.offset, col.transform.lossyScale);

        Vector2 overlapCenter = (Vector2)snapPos + realOffset;

        debugSnapPos = overlapCenter;
        debugBoxSize = realSize;
        Debug.Log(debugSnapPos + " " + debugBoxSize);

        Collider2D[] hits = Physics2D.OverlapBoxAll(overlapCenter, realSize, 0f);
        int highestLayer = -1;
        foreach (Collider2D hit in hits)
        {
            Tile otherTile = hit.GetComponent<Tile>();
            if (otherTile != null)
            {
                if (otherTile.layer > highestLayer)
                    highestLayer = otherTile.layer;
            }
        }

        GameObject tile = Instantiate(tilePrefabToSpawn, snapPos, Quaternion.identity);
        Tile tileScript = tile.GetComponent<Tile>();

        tileScript.layer = highestLayer + 1;
        tileScript.tileData = tileData;

        var foodSprite = tile.transform.Find("Container/Food").GetComponent<SpriteRenderer>();
        foodSprite.sprite = tileData.sprite;

        tile.transform.localScale = Vector3.one;
        tileScript.transform.SetParent(grid);
        Debug.Log("After SetParent: " + tileScript.transform.parent.name);
        Destroy(dragContainer);
        dragContainer = null;

        LevelEditorManager.Instance.isChanged = true;
        SortTiles.Sort(grid.GetComponentsInChildren<Tile>().ToList());
        SortTiles.ActivateShadows(grid.GetComponentsInChildren<Tile>().ToList());
    }


    private void OnDrawGizmos()
    {
        if (debugBoxSize == Vector2.zero) return;

        Vector3 fixedSize = new Vector3(debugBoxSize.x, debugBoxSize.y, 0.01f);

        // Draw yellow box
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(debugSnapPos, fixedSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(debugSnapPos, fixedSize);

        // Draw overlapping colliders (red)
        Gizmos.color = Color.red;
        Collider2D[] hits = Physics2D.OverlapBoxAll(debugSnapPos, debugBoxSize, 0f);
        foreach (var h in hits)
        {
            if (h != null)
            {
                Bounds b = h.bounds;
                Gizmos.DrawWireCube(b.center, b.size);
            }
        }
    }


}
