using UnityEngine;

[CreateAssetMenu(menuName ="Food",fileName ="New Food")]
public class TileDataSO : ScriptableObject
{
    public int id;
    public string foodName;
    public Sprite sprite;
}
