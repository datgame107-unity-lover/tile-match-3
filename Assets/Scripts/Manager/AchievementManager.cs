using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
class AchievementSaveData
{
    public List<AchivementProgress> progresses = new();
}

[Serializable]
public class AchivementProgress
{
    public string id;
    public int current;
    public bool isClaimed;

    public AchivementProgress(string id)
    {
        this.id = id;
        current = 0;
        isClaimed = false;
    }
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [Header("Achievement Data")]
    public List<AchievementData> achivementDatas;

    private Dictionary<string, AchivementProgress> progresses = new();

    private const string SAVE_KEY = "ACHIVEMENT_PROGRESS";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchivements();
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventManager.OnCurrencyChanged += HandleCurrencyChanged ;
        EventManager.OnPlayerWon += HandlePlayerWon ;
        EventManager.OnPlayerLost+=HandlePlayerLost;
        EventManager.OnTilesRemoved+=HandleTilesRemoved;
        EventManager.OnShuffleUsed +=HandleShuffleUsed;
        EventManager.OnHintUsed +=HandleHintUsed;
        EventManager.OnPowerUpUsed += HandlePowerUpUsed ;
        EventManager.OnUndoUsed += HandleUndoUsed;
        EventManager.OnQuestClaimed += HandleQuestClaimed; ;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyChanged -= HandleCurrencyChanged;
        EventManager.OnPlayerLost -= HandlePlayerLost;
    }
    private void HandleCurrencyChanged(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.Flower:
                AddProgress(AchievementType.EarnFlower, amount);
                break;
            case CurrencyType.Diamond:
                AddProgress(AchievementType.EarnDiamond, amount);
                break;
            case CurrencyType.Heart:
                AddProgress(AchievementType.LoseHeart, 1);
                break;

        }
    }
    private void HandlePlayerWon()
    {
        AddProgress(AchievementType.ClearLevel, 1);
    }
    private void HandlePlayerLost()
    {
        AddProgress(AchievementType.LoseHeart, 1);
    }
    private void HandleTilesRemoved(TileDataSO tile = null)
    {
        AddProgress(AchievementType.MatchTiles, 2);
    }
    private void HandleShuffleUsed()
    {
        AddProgress(AchievementType.UseBooster, 1);
    }
    private void HandleHintUsed()
    {
        AddProgress(AchievementType.UseBooster, 1);

    }
    private void HandleUndoUsed()
    {
        AddProgress(AchievementType.UseBooster, 1);

    }
    private void HandlePowerUpUsed()
    {
        AddProgress(AchievementType.UseBooster, 1);

    }
    private void HandleQuestClaimed(QuestDataSO quest = null)
    {
        AddProgress(AchievementType.CompleteQuest, 1);
    }
    #region UPDATE PROGRESS

    public void AddProgress(AchievementType type, int amount)
    {
        foreach (var data in achivementDatas)
        {
            if (data.type != type) continue;

            var progress = GetProgress(data.id);

            if (progress.isClaimed) continue;
            if (progress.current >= data.target) continue;

            progress.current += amount;
            progress.current = Mathf.Min(progress.current, data.target);
        }

        SaveAchivements();
    }

    #endregion

    #region CLAIM

    public void ClaimAchievement(string id)
    {
        var data = achivementDatas.Find(a => a.id == id);
        if (data == null) return;

        var progress = GetProgress(id);

        if (progress.isClaimed) return;
        if (progress.current < data.target) return;

        progress.isClaimed = true;

        //CurrencyManager.Instance.Add(data.rewardType, data.rewardAmount);

        SaveAchivements();
    }

    #endregion

    #region GETTERS

    public AchivementProgress GetProgress(string id)
    {
        if (!progresses.TryGetValue(id, out var p))
        {
            p = new AchivementProgress(id);
            progresses[id] = p;
        }
        return p;
    }

    public float GetProgressPercent(string id)
    {
        var data = achivementDatas.Find(a => a.id == id);
        if (data == null) return 0;

        var p = GetProgress(id);
        return (float)p.current / data.target;
    }

    public bool IsCompleted(string id)
    {
        var data = achivementDatas.Find(a => a.id == id);
        if (data == null) return false;

        return GetProgress(id).current >= data.target;
    }

    #endregion

    #region SAVE / LOAD

    private void SaveAchivements()
    {
        AchievementSaveData save = new();

        foreach (var p in progresses.Values)
            save.progresses.Add(p);

        PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(save));
    }

    private void LoadAchivements()
    {
        progresses.Clear();

        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            InitEmptyProgress();
            return;
        }

        var save = JsonUtility.FromJson<AchievementSaveData>(
            PlayerPrefs.GetString(SAVE_KEY)
        );

        foreach (var p in save.progresses)
            progresses[p.id] = p;

        // đảm bảo achievement mới thêm vẫn có progress
        InitEmptyProgress();
    }

    private void InitEmptyProgress()
    {
        foreach (var data in achivementDatas)
        {
            if (!progresses.ContainsKey(data.id))
                progresses[data.id] =
                    new AchivementProgress(data.id);
        }
    }

    #endregion
}
