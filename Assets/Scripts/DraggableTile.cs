using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public GameObject tilePrefabToSpawn;
    public TileDataSO tileData;

    [Header("Grid")]
    [SerializeField] private Transform grid; 

    private Canvas canvas;
    private GameObject dragContainer;
    [SerializeField] private float cellSize = 0.8f; // kích thước mỗi cell
    [SerializeField] private bool allowHalfCell = true; // cho phép snap nửa cell
    private void Awake()
    {
       
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

        // Dùng collider thay vì sprite (ổn định hơn)
        BoxCollider2D col = tilePrefabToSpawn.GetComponentInChildren<BoxCollider2D>();
        Vector2 size = Vector2.Scale(col.size, col.transform.lossyScale);

        float stepX = allowHalfCell ? size.x / 2f : size.x;
        float stepY = allowHalfCell ? size.y / 2f : size.y;

        float snapX = Mathf.Floor(worldPos.x / stepX + 0.5f) * stepX;
        float snapY = Mathf.Floor(worldPos.y / stepY + 0.5f) * stepY;

        Vector3 snapPos = new Vector3(snapX, snapY, 0f);

        GameObject tile = Instantiate(tilePrefabToSpawn, snapPos, Quaternion.identity);
        Tile tileScript = tile.GetComponent<Tile>();

        Collider2D[] hits = Physics2D.OverlapBoxAll(snapPos, new Vector2(0.8f, 0.8f), 0f);

        int highestLayer = -1;
        foreach (Collider2D hit in hits)
        {
            Tile otherTile = hit.GetComponent<Tile>();
            if (otherTile != null && otherTile != tileScript)
            {
                if (otherTile.layer > highestLayer)
                    highestLayer = otherTile.layer;
            }
        }

        tileScript.layer = highestLayer + 1;

        tile.GetComponent<Tile>().tileData = tileData;
        var food = tile.transform.Find("Container/Food").GetComponent<SpriteRenderer>();
        food.sprite = tileData.sprite;
        tile.transform.localScale = Vector3.one;
        tile.transform.SetParent(grid);
        Destroy(dragContainer);
        dragContainer = null;
    }

}
