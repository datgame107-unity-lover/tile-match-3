using UnityEngine;
using System.Collections.Generic;

public class LevelCreateManager : MonoBehaviour
{
    [Header("Shape Config")]
    public BaseShapeSO shape;

    [Header("Tile Settings")]
    public GameObject tilePrefab;
    public bool useSortingOrder = true; // false = dùng z-position

    [Header("Clear Before Generate")]
    public bool autoClearOldTiles = true;

    public void Generate()
    {
        if (shape == null || tilePrefab == null)
        {
            Debug.LogWarning("⚠️ Missing Shape or Tile Prefab!");
            return;
        }

        if (autoClearOldTiles)
            ClearGeneratedTiles();

        Vector2[] basePositions = shape.GetTilePositions();

        for (int layer = 0; layer < shape.layerCount; layer++)
        {
            foreach (Vector2 basePos in basePositions)
            {
                Vector2 spawnPos = basePos;

                if (!shape.allowOverlap)
                    spawnPos += shape.layerOffset * layer;

                Tile tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform).GetComponent<Tile>();
                tile.layer = layer; 
                if (tile.layer == shape.layerCount - 1)
                    tile.isBlocked = false;
                else
                    tile.isBlocked = true;
            }
        }
    }

    public void ClearGeneratedTiles()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
            children.Add(child);

        foreach (Transform t in children)
            DestroyImmediate(t.gameObject);
    }
}
