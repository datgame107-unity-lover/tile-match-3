using System.Collections.Generic;
using UnityEngine;

public class InfiniteModeHandler : IGameModeHandler
{
    private TileManager manager;

    public InfiniteModeHandler(TileManager manager)
    {
        this.manager = manager;
    }

    public void OnTileSelected(Tile tile)
    {
        manager.DefaultSelectLogic(tile);

        // Nếu còn ít tile thì sinh thêm layer mới
        if (manager.currentTiles.Count < 15)
        {
             List<Tile> generatedTiles = LevelManager.Instance.GenerateOneLayer(Random.Range(1,4), 30);
            manager.SortTileAndActivateShadow(generatedTiles);
        }
    }

    public void OnTilesMatched(TileDataSO tileData)
    {
        EventManager.OnTileRemoved?.Invoke(tileData);
    }

    public void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles)
    {
        // Infinite mode không bao giờ win
    }
}
