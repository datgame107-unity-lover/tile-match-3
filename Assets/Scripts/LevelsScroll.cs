using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelsScroll : MonoBehaviour
{
    public Transform grid;
    public GameObject tilePrefab;

    private int currentLevel;

    private void OnEnable()
    {
        EventManager.OnChoseLevel += ChoseLevelHandler;
    }

    private void OnDisable()
    {
        EventManager.OnChoseLevel -= ChoseLevelHandler;

    }

    private void ChoseLevelHandler(int level)
    {
        ClearGrid(grid);
        if (LevelDataManager.GetTotalLevel() < level)
        {
            return;
        }
        if (level != currentLevel)
        {
            currentLevel = level;
        }
        LoadLevel(currentLevel);

    }

    private void LoadLevel(int level)
    {
        LevelDataManager.LoadFromSO(level, tilePrefab, grid);
    }
    private List<Tile> GetTile(Transform grid)
    {
        return grid.transform.GetComponentsInChildren<Tile>().ToList();
    }

    private void ClearGrid(Transform grid)
    {
        foreach (var tile in GetTile(grid))
        {
            Destroy(tile.gameObject);
        }
    }

   
}
