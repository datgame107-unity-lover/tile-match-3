using System.Collections.Generic;
using UnityEngine;

public class LevelDataManager
{
    private readonly Dictionary<int, LevelSaveData> _levels;

    public LevelDataManager(List<LevelSaveData> levels)
    {
        _levels = new Dictionary<int, LevelSaveData>();

        foreach (var l in levels)
            _levels[l.levelIndex] = l;
    }

    public LevelSaveData GetLevel(int index)
    {
        if (_levels.TryGetValue(index, out var level))
            return level;

        Debug.LogWarning($"Level {index} not found");
        return null;
    }
}