using System;
using UnityEngine;

[Serializable]
public class RuntimeTileData  
{
        public int tileId;      // lấy từ TileDataSO.id  
        public Vector3 worldPos;
        public int layer;
        public bool isBlocked;
        public bool isClicked;
        public bool shadow;
    }

