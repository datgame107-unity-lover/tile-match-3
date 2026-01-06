using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public int width = 5;
    public int height = 7;
    public List<TileSaveData> tiles = new();
}


