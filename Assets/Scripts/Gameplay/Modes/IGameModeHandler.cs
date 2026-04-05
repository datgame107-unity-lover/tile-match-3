// Scripts/Gameplay/Modes/IGameModeHandler.cs
using System.Collections.Generic;

public interface IGameModeHandler
{
    void Initialize();
    void OnPlayOn();
    void OnTileSelected(Tile tile);
    void OnTilesMatched(TileDataSO tileData, Tile tile);
    void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles);
    void OnResetLevel();
    void OnContinueLevel();
}