// Scripts/Data/SO/LevelDataSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public int levelIndex;
    public List<TileEntry> tiles;

    [System.Serializable]
    public class TileEntry
    {
        public TileDataSO tileData;
        public int layer;
        public Vector2 position;
    }
}