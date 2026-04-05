using UnityEngine;

public class LevelMapper
{
    public static LevelRuntimeData FromSave(
        LevelSaveData save,
        TileDatabaseSO db)
    {
        var runtime = new LevelRuntimeData();

        foreach (var t in save.tiles)
        {
            runtime.tiles.Add(new RuntimeTileData
            {
                worldPos = new Vector3(t.x, t.y, 0),
                tileData = db.Get(t.tileId),
                layer = t.layer
            });
        }

        return runtime;
    }
}