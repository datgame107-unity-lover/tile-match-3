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
        List<Tile> tileList = LevelDataManager.LoadFromSORuntime(PlayerPrefs.GetInt("level"), manager.tilePrefab, manager.transform);
        manager.currentTiles = tileList;
    }

   
    public void OnPlayOn()
    {
    }

    public void OnResetLevel()
    {   
        manager.selectingTiles = new List<Tile>();
        manager.currentTiles =  LevelDataManager.LoadFromSORuntime(PlayerPrefs.GetInt("level"), manager.tilePrefab, manager.transform);

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
        LevelDataManager.LoadFromSORuntime(PlayerPrefs.GetInt("level"), manager.tilePrefab, manager.transform);
        manager. RefreshCurrentTilesFromHierarchy();
        GameManager.instance.ChangeState(GameState.Playing);
    }

  
}
