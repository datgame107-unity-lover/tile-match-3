using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tile Database")]
public class TileDatabaseSO : ScriptableObject
{
    public List<TileDataSO> tiles;

    private Dictionary<string, TileDataSO> lookup;

    public void Init()
    {
        lookup = new();

        foreach (var t in tiles)
            lookup[t.tileId] = t;
    }

    public TileDataSO Get(string id)
    {
        if (lookup == null)
        {
            Debug.LogError("[TileDatabaseSO] Init() chưa được gọi!");
            return null;
        }

        if (!lookup.TryGetValue(id, out var data))
        {
            Debug.LogError($"Tile id not found: {id}");
            return null;
        }
        return data;
    }
}