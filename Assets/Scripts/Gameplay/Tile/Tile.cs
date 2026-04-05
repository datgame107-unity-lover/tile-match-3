// Scripts/Gameplay/Tile/Tile.cs
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Data")]
    public TileDataSO tileData;
    public int layer;
    public bool isBlocked;
    public bool isClicked;
    public Vector2 worldPos;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        worldPos = transform.position;
    }

    // ── Init (gọi từ LevelManager.SpawnTiles) ────────────
    public void Init(TileDataSO data, int tileLayer)
    {
        tileData = data;
        layer = tileLayer;
        ApplyData();
        UpdateSortingOrder();
    }

    public Vector3 GetOriginalScale() => originalScale;

    public void ApplyData()
    {
        if (tileData == null) return;
        var food = transform.Find("Container/Food");
        if (food != null)
            food.GetComponent<SpriteRenderer>().sprite = tileData.sprite;
    }

   public void UpdateSortingOrder()
{
    int baseOrder = layer * 10;

    var base_ = transform.Find("Container/Base");
    if (base_ != null)
    {
        // base background = baseOrder + 0
        var baseRenderer = base_.GetComponent<SpriteRenderer>();
        if (baseRenderer != null)
            baseRenderer.sortingOrder = baseOrder;
    }

    var food = transform.Find("Container/Food");
    if (food != null)
        food.GetComponent<SpriteRenderer>().sortingOrder = baseOrder + 1;

    var shadow = transform.Find("Container/Shadow");
    if (shadow != null)
        shadow.GetComponent<SpriteRenderer>().sortingOrder = baseOrder + 2;
}

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
        var shadow = transform.Find("Container/Shadow");
        if (shadow != null)
            shadow.gameObject.SetActive(blocked);
    }
}