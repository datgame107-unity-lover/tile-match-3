using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public List<TileSaveData> tiles;
}
