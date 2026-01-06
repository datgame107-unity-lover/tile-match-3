using System;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
    #region === DATA ===
    [Header("Data")]
    public TileDataSO tileData;
    public Vector3 worldPos;
    public int layer;                  // Layer cao = ở trên
    public bool isBlocked = true;
    public bool isClicked;
    #endregion

    #region === REFS ===
    [Header("Refs")]
    [SerializeField] private TileUI ui;
    private SpriteRenderer[] renderers;
    #endregion

    #region === UNITY ===
    private void Awake()
    {
        if (ui == null)
            ui = GetComponentInChildren<TileUI>();
       
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }
    #endregion

    #region === SORTING ORDER ===
    public void UpdateSortingOrder()
    {
        float y = transform.position.y;

        int baseOrder = layer * 1000 + Mathf.RoundToInt(-y * 100);
        int order = baseOrder;

        foreach (var r in renderers)
        {
            r.sortingOrder = order;
            order++;
        }
    }
    public Vector3 GetOriginalScale()
    {
        return ui.OriginalScale;
    }
    #endregion

    #region === POSITION ===
    public void SetWorldPosition(Vector3 pos)
    {
        worldPos = pos;
        transform.position = pos;
        UpdateSortingOrder();
    }
    #endregion

    #region === DATA ===
    public void ApplyData()
    {
        if (tileData == null) return;

        ui.SetSprite(tileData.sprite);
    }

    public void SwapData(Tile other)
    {
        (tileData, other.tileData) = (other.tileData, tileData);

        ApplyData();
        other.ApplyData();
    }
    #endregion

    #region === LAYER & VISUAL ===
    public void SetLayer(int newLayer)
    {
        layer = newLayer;
        UpdateSortingOrder();
    }

    public void UpdateVisual(int maxLayer)
    {
        ui.UpdateLayerVisual(layer, maxLayer);
    }

    public void SetShadow(bool value)
    {
        ui.SetShadow(value);
    }
    #endregion
}
