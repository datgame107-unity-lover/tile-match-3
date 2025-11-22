using System.Collections.Generic;

public interface IGameModeHandler
{
    void OnTileSelected(Tile tile);
    void OnTilesMatched(TileDataSO tileData,Tile tile);
    void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles);
}
