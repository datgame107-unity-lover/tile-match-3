// Scripts/Data/SaveData/SaveStructs.cs
using System;
using System.Collections.Generic;
using UnityEngine;

// ── Tile ──────────────────────────────────────────────
[Serializable]
public class TileSaveData
{
    public TileDataSO tile;
    public Vector3 worldPos;
    public int layer;
    public bool isBlocked;
    public bool clicked;
    public bool shadow;
}

// ── Quest ─────────────────────────────────────────────
[Serializable]
public class QuestSaveData
{
    public string id;
    public int amount;
    public bool claimed;
}

[Serializable]
public class QuestProgressSave
{
    public List<QuestSaveData> quests = new();
    public int dailyWinCount;
    public bool dailyQuestChestClaimed;
    public bool dailyWinChestClaimed;
}

// ── Achievement ───────────────────────────────────────
[Serializable]
public class AchievementSaveEntry
{
    public string id;
    public int current;
    public bool isClaimed;
}

[Serializable]
public class AchievementProgressSave
{
    public List<AchievementSaveEntry> entries = new();
}

// ── Progress runtime (không serialize) ───────────────
public class ProgressData
{
    public string id;
    public int currentAmount;
    public bool isClaimed;

    public ProgressData(string id)
    {
        this.id = id;
    }
}

public class AchievementProgress
{
    public string id;
    public int current;
    public bool isClaimed;

    public AchievementProgress(string id)
    {
        this.id = id;
    }
}