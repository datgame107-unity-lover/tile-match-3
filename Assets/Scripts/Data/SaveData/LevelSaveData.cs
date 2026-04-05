// Scripts/Data/SaveData/LevelSaveData.cs
using System.Collections.Generic;

[System.Serializable]
public class LevelSaveData
{
    public int levelIndex;
    public List<TilePlacement> tiles = new List<TilePlacement>();
}

[System.Serializable]
public class TilePlacement
{
    public string tileId;
    public float x;
    public float y;
    public int layer;
}