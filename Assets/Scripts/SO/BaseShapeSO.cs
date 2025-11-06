using UnityEngine;

public abstract class BaseShapeSO : ScriptableObject
{
    [Header("Shape Setup")]
    public float scale = 1f;

    [Header("Layer Settings")]
    public int tilesPerLayer = 10;
    public int layerCount = 1;

    [Header("Overlap / Offset Settings")]
    public bool allowOverlap = true;
    public Vector2 layerOffset = new Vector2(0.2f, 0.2f);

    public abstract Vector2[] GetTilePositions();
}
