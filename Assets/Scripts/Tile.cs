using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Tile : MonoBehaviour
{
    public TileDataSO tileData;
    public Vector3 worldPos;
    public Vector2Int gridPos;
    public int layer;
    public bool isBlocked = true;
    public bool isClicked;

    
    

   
}
