using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
    [Header("Data")]
    public TileDataSO tileData;
    public Vector3 worldPos;
    public int layer;              // Z-logic của tile
    public bool isBlocked = true;
    public bool isClicked;

    private SpriteRenderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void UpdateSortingOrder()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SpriteRenderer>();

        float y = transform.position.y;

        int baseOrder = layer * 1000 + (int)(-y * 100);

        foreach (var r in renderers)
        {
            int order = baseOrder;

            if (r.gameObject.name.ToLower().Contains("shadow"))
                order -= 1; // shadow nằm dưới 1 lớp

            r.sortingOrder = order;
        }
    }

    public void AddLayer(int amount)
    {
        layer += amount;
        UpdateSortingOrder();
    }

    public void SetWorldPosition(Vector3 pos)
    {
        transform.position = pos;
        worldPos = pos;
        UpdateSortingOrder();
    }
}
