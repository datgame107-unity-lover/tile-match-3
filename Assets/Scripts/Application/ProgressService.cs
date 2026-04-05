// Scripts/Application/ProgressService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressService
{
    // ── Quest ─────────────────────────────────────────
    private readonly List<QuestDataSO> dailyQuests;
    private readonly List<QuestReward> dailyQuestRewards;
    private readonly List<QuestReward> dailyWinRewards;
    private readonly int dailyWinTarget;
    private Dictionary<string, ProgressData> questProgresses = new();
    private int dailyWinCount;
    private bool dailyQuestChestClaimed;
    private bool dailyWinChestClaimed;

    // ── Achievement ───────────────────────────────────
    private readonly List<AchievementDataSO> achievements;
    private Dictionary<string, AchievementProgress> achvProgresses = new();

    private readonly ISaveService save;
    private readonly CurrencyService currency;

    public ProgressService(
        ISaveService save,
        CurrencyService currency,
        List<QuestDataSO> dailyQuests,
        List<QuestReward> dailyQuestRewards,
        List<QuestReward> dailyWinRewards,
        List<AchievementDataSO> achievements,
        int dailyWinTarget = 5)
    {
        this.save = save;
        this.currency = currency;
        this.dailyQuests = dailyQuests;
        this.dailyQuestRewards = dailyQuestRewards;
        this.dailyWinRewards = dailyWinRewards;
        this.achievements = achievements;
        this.dailyWinTarget = dailyWinTarget;

        Load();
        CheckDailyReset();
        SubscribeEvents();
    }

    // ── Quest public API ──────────────────────────────
    public ProgressData GetQuestProgress(string id)
    {
        if (!questProgresses.TryGetValue(id, out var p))
        {
            p = new ProgressData(id);
            questProgresses[id] = p;
        }
        return p;
    }

    public void ClaimQuest(string questID)
    {
        if (!questProgresses.TryGetValue(questID, out var progress)) return;

        var quest = dailyQuests.Find(q => q.questID == questID);
        if (quest == null) return;
        if (progress.currentAmount < quest.targetAmount) return;
        if (progress.isClaimed) return;

        progress.isClaimed = true;

        foreach (var reward in quest.rewards)
            currency.Add(reward.type, reward.amount);

        Save();
        EventBus<QuestClaimedEvent>.Publish(new QuestClaimedEvent { quest = quest });
    }

    public void ClaimDailyQuestChest()
    {
        if (!IsDailyQuestReady()) return;
        if (dailyQuestChestClaimed) return;

        dailyQuestChestClaimed = true;

        foreach (var reward in dailyQuestRewards)
            currency.Add(reward.type, reward.amount);

        Save();
    }

    public void ClaimDailyWinChest()
    {
        if (!IsDailyWinReady()) return;
        if (dailyWinChestClaimed) return;

        dailyWinChestClaimed = true;

        foreach (var reward in dailyWinRewards)
            currency.Add(reward.type, reward.amount);

        Save();
    }

    public bool IsDailyQuestReady() => questProgresses.Values.All(p => p.isClaimed);
    public bool IsDailyQuestChestClaimed() => dailyQuestChestClaimed;
    public bool IsDailyWinReady() => dailyWinCount >= dailyWinTarget;
    public bool IsDailyWinChestClaimed() => dailyWinChestClaimed;
    public int GetDailyWinCount() => dailyWinCount;
    public float GetDailyWinProgress() => (float)dailyWinCount / dailyWinTarget;
    public int GetClaimedQuestCount() => questProgresses.Values.Count(p => p.isClaimed);
    public List<QuestDataSO> GetDailyQuests() => dailyQuests;
    public int GetTotalDailyQuest() => dailyQuests.Count;
    public float GetDailyQuestProgress() => (float)GetClaimedQuestCount() / GetTotalDailyQuest();
    public List<QuestReward> GetDailyQuestRewards() => dailyQuestRewards;
    public List<QuestReward> GetDailyWinRewards() => dailyWinRewards;

    // ── Achievement public API ────────────────────────
    public AchievementProgress GetAchievementProgress(string id)
    {
        if (!achvProgresses.TryGetValue(id, out var p))
        {
            p = new AchievementProgress(id);
            achvProgresses[id] = p;
        }
        return p;
    }

    public float GetAchievementPercent(string id)
    {
        var data = achievements.Find(a => a.id == id);
        if (data == null) return 0f;
        return (float)GetAchievementProgress(id).current / data.target;
    }

    public bool IsAchievementCompleted(string id)
    {
        var data = achievements.Find(a => a.id == id);
        if (data == null) return false;
        return GetAchievementProgress(id).current >= data.target;
    }

    public void ClaimAchievement(string id)
    {
        var data = achievements.Find(a => a.id == id);
        if (data == null) return;

        var progress = GetAchievementProgress(id);
        if (progress.isClaimed) return;
        if (progress.current < data.target) return;

        progress.isClaimed = true;
        currency.Add(data.rewardType, data.rewardAmount);

        Save();
        EventBus<AchievementUnlockedEvent>.Publish(
            new AchievementUnlockedEvent { data = data });
    }

    public void Dispose()
    {
        EventBus<TilesRemovedEvent>.Unsubscribe(OnTilesRemoved);
        EventBus<PlayerWonEvent>.Unsubscribe(OnPlayerWon);
        EventBus<ShuffleUsedEvent>.Unsubscribe(OnShuffleUsed);
        EventBus<HintUsedEvent>.Unsubscribe(OnHintUsed);
        EventBus<UndoUsedEvent>.Unsubscribe(OnUndoUsed);
        EventBus<PowerUpUsedEvent>.Unsubscribe(OnPowerUpUsed);
        EventBus<QuestClaimedEvent>.Unsubscribe(OnQuestClaimed);
        EventBus<HighScoreChangedEvent>.Unsubscribe(OnHighScoreChanged);
        EventBus<CurrencyChangedEvent>.Unsubscribe(OnCurrencyChanged);
    }

    // ── Event handlers ────────────────────────────────
    private void SubscribeEvents()
    {
        EventBus<TilesRemovedEvent>.Subscribe(OnTilesRemoved);
        EventBus<PlayerWonEvent>.Subscribe(OnPlayerWon);
        EventBus<ShuffleUsedEvent>.Subscribe(OnShuffleUsed);
        EventBus<HintUsedEvent>.Subscribe(OnHintUsed);
        EventBus<UndoUsedEvent>.Subscribe(OnUndoUsed);
        EventBus<PowerUpUsedEvent>.Subscribe(OnPowerUpUsed);
        EventBus<QuestClaimedEvent>.Subscribe(OnQuestClaimed);
        EventBus<HighScoreChangedEvent>.Subscribe(OnHighScoreChanged);
        EventBus<CurrencyChangedEvent>.Subscribe(OnCurrencyChanged);
    }

    private void OnPlayerWon(PlayerWonEvent _)
    {
        dailyWinCount = Mathf.Clamp(dailyWinCount + 1, 0, dailyWinTarget);
        UpdateQuestProgress(QuestType.CompleteLevels, 1);
        AddAchievementProgress(AchievementType.ClearLevel, 1);
        Save();
    }

    private void OnTilesRemoved(TilesRemovedEvent e)
    {
        UpdateQuestProgress(QuestType.CollectItem, 1, e.tileData);
        AddAchievementProgress(AchievementType.MatchTiles, 1);
    }

    private void OnShuffleUsed(ShuffleUsedEvent _)
    {
        UpdateQuestProgress(QuestType.UseAbility, 1);
        AddAchievementProgress(AchievementType.UseBooster, 1);
    }

    private void OnHintUsed(HintUsedEvent _)
    {
        UpdateQuestProgress(QuestType.UseAbility, 1);
        AddAchievementProgress(AchievementType.UseBooster, 1);
    }

    private void OnUndoUsed(UndoUsedEvent _)
    {
        UpdateQuestProgress(QuestType.UseAbility, 1);
        AddAchievementProgress(AchievementType.UseBooster, 1);
    }

    private void OnPowerUpUsed(PowerUpUsedEvent _)
    {
        UpdateQuestProgress(QuestType.UseAbility, 1);
        AddAchievementProgress(AchievementType.UseBooster, 1);
    }

    private void OnQuestClaimed(QuestClaimedEvent _)
    {
        AddAchievementProgress(AchievementType.CompleteQuest, 1);
    }

    private void OnHighScoreChanged(HighScoreChangedEvent e)
    {
        UpdateAchievementProgress(AchievementType.HighScore, e.score);
    }

    private void OnCurrencyChanged(CurrencyChangedEvent e)
    {
        switch (e.type)
        {
            case CurrencyType.Flower:
                AddAchievementProgress(AchievementType.EarnFlower, e.amount);
                break;
            case CurrencyType.Diamond:
                AddAchievementProgress(AchievementType.EarnDiamond, e.amount);
                break;
        }
    }

    // ── Quest progress logic ──────────────────────────
    private void UpdateQuestProgress(
        QuestType type, int amount, TileDataSO tileTarget = null)
    {
        foreach (var q in dailyQuests)
        {
            if (q.type != type) continue;
            if (type == QuestType.CollectItem && q.targetTile != tileTarget) continue;

            if (!questProgresses.TryGetValue(q.questID, out var p))
            {
                p = new ProgressData(q.questID);
                questProgresses[q.questID] = p;
            }

            if (p.isClaimed) continue;

            int before = p.currentAmount;
            p.currentAmount = Mathf.Min(p.currentAmount + amount, q.targetAmount);

            if (before < q.targetAmount && p.currentAmount >= q.targetAmount)
                EventBus<QuestCompletedEvent>.Publish(
                    new QuestCompletedEvent { quest = q });
        }

        Save();
    }

    // ── Achievement progress logic ────────────────────
    private void AddAchievementProgress(AchievementType type, int amount)
    {
        foreach (var data in achievements)
        {
            if (data.type != type) continue;

            var p = GetAchievementProgress(data.id);
            if (p.isClaimed) continue;
            if (p.current >= data.target) continue;

            p.current = Mathf.Min(p.current + amount, data.target);
        }

        Save();
    }

    private void UpdateAchievementProgress(AchievementType type, int value)
    {
        foreach (var data in achievements)
        {
            if (data.type != type) continue;

            var p = GetAchievementProgress(data.id);
            if (p.isClaimed) continue;
            if (p.current >= data.target) continue;

            p.current = Mathf.Min(value, data.target);
        }

        Save();
    }

    // ── Daily reset ───────────────────────────────────
    private void CheckDailyReset()
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        string lastDay = save.GetString(SaveKeys.Quest.Day, "");

        if (lastDay == today) return;

        ResetDailyData();
        save.SetString(SaveKeys.Quest.Day, today);
        save.Save();
    }

    private void ResetDailyData()
    {
        questProgresses.Clear();
        foreach (var q in dailyQuests)
            questProgresses[q.questID] = new ProgressData(q.questID);

        dailyQuestChestClaimed = false;
        dailyWinChestClaimed = false;
        dailyWinCount = 0;

        Save();
    }

    // ── Save / Load ───────────────────────────────────
    private void Save()
    {
        // Quest
        var questSave = new QuestProgressSave
        {
            dailyWinCount = dailyWinCount,
            dailyQuestChestClaimed = dailyQuestChestClaimed,
            dailyWinChestClaimed = dailyWinChestClaimed,
        };

        foreach (var kvp in questProgresses)
        {
            questSave.quests.Add(new QuestSaveData
            {
                id = kvp.Key,
                amount = kvp.Value.currentAmount,
                claimed = kvp.Value.isClaimed,
            });
        }

        save.SetString(SaveKeys.Quest.Day,
            JsonUtility.ToJson(questSave));

        // Achievement
        var achvSave = new AchievementProgressSave();
        foreach (var kvp in achvProgresses)
        {
            achvSave.entries.Add(new AchievementSaveEntry
            {
                id = kvp.Key,
                current = kvp.Value.current,
                isClaimed = kvp.Value.isClaimed,
            });
        }

        save.SetString(SaveKeys.Achievement.Progress,
            JsonUtility.ToJson(achvSave));

        save.Save();
    }

    private void Load()
    {
        LoadQuests();
        LoadAchievements();
    }

    private void LoadQuests()
    {
        questProgresses.Clear();

        if (!save.HasKey(SaveKeys.Quest.Progress)) 
        {
            ResetDailyData();
            return;
        }

        var data = JsonUtility.FromJson<QuestProgressSave>(
            save.GetString(SaveKeys.Quest.Progress));

        foreach (var q in dailyQuests)
        {
            var saved = data.quests.Find(x => x.id == q.questID);
            questProgresses[q.questID] = saved != null
                ? new ProgressData(q.questID)
                {
                    currentAmount = saved.amount,
                    isClaimed = saved.claimed,
                }
                : new ProgressData(q.questID);
        }

        dailyWinCount = data.dailyWinCount;
        dailyQuestChestClaimed = data.dailyQuestChestClaimed;
        dailyWinChestClaimed = data.dailyWinChestClaimed;
    }

    private void LoadAchievements()
    {
        achvProgresses.Clear();

        if (!save.HasKey(SaveKeys.Achievement.Progress))
        {
            InitEmptyAchievements();
            return;
        }

        var data = JsonUtility.FromJson<AchievementProgressSave>(
            save.GetString(SaveKeys.Achievement.Progress));

        foreach (var entry in data.entries)
            achvProgresses[entry.id] = new AchievementProgress(entry.id)
            {
                current = entry.current,
                isClaimed = entry.isClaimed,
            };

        InitEmptyAchievements();
    }

    private void InitEmptyAchievements()
    {
        foreach (var data in achievements)
        {
            if (!achvProgresses.ContainsKey(data.id))
                achvProgresses[data.id] = new AchievementProgress(data.id);
        }
    }
    public List<AchievementDataSO> GetAchievements() => achievements;
}