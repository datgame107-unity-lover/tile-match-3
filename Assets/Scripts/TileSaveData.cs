using UnityEngine;

[System.Serializable]
public class TileSaveData
{
    public TileDataSO tile;
    public Vector3 worldPos;
    public Vector2Int gridPos;
    public int layer;
    public bool isBlocked;
    public bool clicked;
}