using UnityEngine;

[CreateAssetMenu (fileName = "Tile Data", menuName = "Game/Tile Data")]
public class TileDataSO : ScriptableObject
{
    public string tileId;
    public string tileName;
    public Sprite sprite;
}
