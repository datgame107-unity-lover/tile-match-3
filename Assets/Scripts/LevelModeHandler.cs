using System.Collections.Generic;

public class LevelModeHandler : IGameModeHandler
{
    private TileManager manager;

    public LevelModeHandler(TileManager manager)
    {
        this.manager = manager;
    }

    public void OnTileSelected(Tile tile)
    {
        manager.DefaultSelectLogic(tile);
    }

    public void OnTilesMatched(TileDataSO tileData)
    {
        EventManager.OnTileRemoved?.Invoke(tileData);
    }

    public void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles)
    {
        if (currentTiles.Count == 0 && selectingTiles.Count == 0)
        {
            manager.StartCoroutine(manager.Win());
        }
    }
}
