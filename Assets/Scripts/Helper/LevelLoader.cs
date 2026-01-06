using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelDataSO level;
    public Transform gridParent;
    public GameObject tilePrefab;

    void Start()
    {
        foreach (var t in level.tiles)
        {
            Vector3 pos = new(
                t.gridPos.x * 0.8f,
                t.gridPos.y * 0.8f,
                0);

            GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, gridParent);
            Tile tileScript = tile.GetComponent<Tile>();

            tileScript.layer = t.layer;
            tileScript.tileData = t.tile;
        }
    }
}
