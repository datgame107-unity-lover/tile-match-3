using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class LevelModeHandler : IGameModeHandler
{
    private TileManager manager;

    public LevelModeHandler(TileManager manager)
    {
        this.manager = manager;
    }
    public void Initialize()
    {
        GameManager.instance.ChangeMode(GameMode.Level);
        List<Tile> tileList =  LevelDataManager.LoadFromSO(PlayerPrefs.GetInt("level"), manager.tilePrefab, manager.transform);
        manager.currentTiles = tileList;
        manager.SortTileAndActivateShadow();
    }

    private IEnumerator SortNextFrame()
    {
        yield return null; // chờ Unity update hierarchy
        manager.SortTileAndActivateShadow();
    }
    public void OnPlayOn()
    {
    }

    public void OnResetLevel()
    {   
        manager.selectingTiles = new List<Tile>();
        manager.currentTiles =  LevelDataManager.LoadFromSO(PlayerPrefs.GetInt("level"), manager.tilePrefab, manager.transform);
        manager.SortTileAndActivateShadow();

    }

  
    public void OnTileSelected(Tile tile)
    {
        manager.DefaultSelectLogic(tile);
    }

    public void OnTilesMatched(TileDataSO tileData, Tile tile)
    {
        EventManager.OnTilesRemoved?.Invoke(tileData);
    }

    public void OnWinCheck(List<Tile> currentTiles, List<Tile> selectingTiles)
    {
        if (currentTiles.Count == 0 && selectingTiles.Count == 0)
        {
            manager.StartCoroutine(manager.Win());
        }
    }

    public void OnContinueLevel()
    {
        LevelDataManager.LoadFromSO(PlayerPrefs.GetInt("level"), manager.tilePrefab, manager.transform);
        manager. RefreshCurrentTilesFromHierarchy();
        manager. SortTileAndActivateShadow();
        GameManager.instance.ChangeState(GameState.Playing);
    }

  
}
