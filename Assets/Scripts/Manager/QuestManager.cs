using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Reset Config")]
    public bool ResetWhenStart = false;

    [Header("Daily Quest Data")]
    public List<QuestDataSO> dailyQuestsData;

    [Header("Daily Win Config")]
    public int dailyWinTarget = 5;
    private int dailyWinCount = 0;

    // Chest Claim Tracking
    private bool dailyQuestChestClaimed = false;
    private bool dailyWinChestClaimed = false;

    // Runtime progress for daily quests
    private Dictionary<string, ProgressData> progresses = new();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (ResetWhenStart)
            {
                ResetDailyData();
            }
            CheckDailyReset();
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        EventManager.OnTilesRemoved += HandleTilesCollected;
        EventManager.OnPlayerWon += HandleLevelWon;
        EventManager.OnPowerUpUsed += HandlePowerUpUsed;
        EventManager.OnHintUsed += HandleHintUsed;
        EventManager.OnShuffleUsed += HandleShuffleUsed;
        EventManager.OnUndoUsed += HandleUndoUsed;
        LoadProgress();

    }

    private void OnDisable()
    {
        EventManager.OnTilesRemoved -= HandleTilesCollected;
        EventManager.OnPlayerWon -= HandleLevelWon;
        EventManager.OnPowerUpUsed -= HandlePowerUpUsed;
        EventManager.OnHintUsed -= HandleHintUsed;
        EventManager.OnShuffleUsed -= HandleShuffleUsed;
        EventManager.OnUndoUsed -= HandleUndoUsed;
    }


    private void HandleLevelWon()
    {
        dailyWinCount++;
        dailyWinCount = Mathf.Clamp(dailyWinCount, 0, dailyWinTarget);

        UpdateQuestProgress(QuestType.CompleteLevels, 1);
        SaveProgress();
    }

    private void HandlePowerUpUsed() => UpdateQuestProgress(QuestType.UseAbility, 1);
    private void HandleUndoUsed() => UpdateQuestProgress(QuestType.UseAbility, 1);
    private void HandleHintUsed() => UpdateQuestProgress(QuestType.UseAbility, 1);
    private void HandleShuffleUsed() => UpdateQuestProgress(QuestType.UseAbility, 1);

    private void HandleTilesCollected(TileDataSO tile)
        => UpdateQuestProgress(QuestType.CollectItem, 1, tile);


    public void UpdateQuestProgress(
     QuestType type,
     int amount,
     TileDataSO tileTarget = null
 )
    {
        foreach (var q in dailyQuestsData)
        {
            if (!progresses.TryGetValue(q.questID, out var p))
            {
                p = new ProgressData(q.questID);
                progresses[q.questID] = p;
            }

            if (p.isClaimed)
                continue;

            if (q.type != type)
                continue;

            if (type == QuestType.CollectItem && q.targetTile != tileTarget)
                continue;

            int before = p.currentAmount;

            p.currentAmount += amount;
            p.currentAmount = Mathf.Min(p.currentAmount, q.targetAmount);

            // ===== QUEST JUST COMPLETED =====
            if (before < q.targetAmount && p.currentAmount >= q.targetAmount)
            {
                EventManager.OnQuestCompleted?.Invoke(q);
            }
        }

        SaveProgress();
    }



    public void ClaimReward(string questID)
    {
        if (!progresses.TryGetValue(questID, out var progress))
            return;
        print("claimed");
        QuestDataSO quest = dailyQuestsData.Find(q => q.questID == questID);
        if (quest == null) return;

        if (progress.currentAmount < quest.targetAmount) return;
        if (progress.isClaimed) return;

        progress.isClaimed = true;

        foreach (var reward in quest.rewards)
            CurrencyManager.Instance.Add(reward.type, reward.amount);

        SaveProgress();
        EventManager.OnQuestClaimed?.Invoke(quest);
    }


    public bool IsDailyQuestReady()
    {
        return progresses.Values.All(p => p.isClaimed);
    }

    public bool IsDailyQuestChestClaimed() => dailyQuestChestClaimed;

    public void ClaimDailyQuestChest()
    {
        if (!IsDailyQuestReady()) return;
        if (dailyQuestChestClaimed) return;

        dailyQuestChestClaimed = true;

        CurrencyManager.Instance.Add(CurrencyType.Flower, 100);

        SaveProgress();
    }


    public int GetDailyWinCount() => dailyWinCount;

    public float GetDailyWinProgress() => (float)dailyWinCount / dailyWinTarget;

    public bool IsDailyWinReady() => dailyWinCount >= dailyWinTarget;

    public bool IsDailyWinChestClaimed() => dailyWinChestClaimed;

    public void ClaimDailyWinChest()
    {
        if (!IsDailyWinReady()) return;
        if (dailyWinChestClaimed) return;

        dailyWinChestClaimed = true;

        // EXAMPLE reward
        CurrencyManager.Instance.Add(CurrencyType.Diamond, 50);

        SaveProgress();
    }


    private void CheckDailyReset()
    {
        string today = System.DateTime.Now.ToString("yyyyMMdd");
        string lastDay = PlayerPrefs.GetString("QUEST_DAY", "");

        if (lastDay != today)
        {
            ResetDailyData();
            PlayerPrefs.SetString("QUEST_DAY", today);
        }
    }

    private void ResetDailyData()
    {
        // Reset quest progresses
        progresses.Clear();
        foreach (var q in dailyQuestsData)
            progresses[q.questID] = new ProgressData(q.questID);

        // Reset chests
        dailyQuestChestClaimed = false;
        dailyWinChestClaimed = false;

        // Reset daily win count
        dailyWinCount = 0;
         
        SaveProgress();
    }


    [System.Serializable]
    class SaveData
    {
        public List<QuestSaveData> quests = new();

        public int dailyWinCount;

        public bool dailyQuestChestClaimed;
        public bool dailyWinChestClaimed;
    }

    [System.Serializable]
    class QuestSaveData
    {
        public string id;
        public int amount;
        public bool claimed;
    }

    private void SaveProgress()
    {
        SaveData data = new ();

        foreach (var kvp in progresses)
        {
            data.quests.Add(new QuestSaveData
            {
                id = kvp.Key,
                amount = kvp.Value.currentAmount,
                claimed = kvp.Value.isClaimed
            });
        }

        data.dailyWinCount = dailyWinCount;
        data.dailyQuestChestClaimed = dailyQuestChestClaimed;
        data.dailyWinChestClaimed = dailyWinChestClaimed;

        PlayerPrefs.SetString("QUEST_PROGRESS", JsonUtility.ToJson(data));
    }

    private void LoadProgress()
    {
        progresses.Clear();

        if (!PlayerPrefs.HasKey("QUEST_PROGRESS"))
        {
            ResetDailyData();
            return;
        }

        var data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("QUEST_PROGRESS"));
        print(data);
        foreach (QuestDataSO q in dailyQuestsData)
        {
            var saved = data.quests.Find(x => x.id == q.questID);
            if (saved != null)
            {
                progresses[q.questID] = new ProgressData(q.questID)
                {
                    currentAmount = saved.amount,
                    isClaimed = saved.claimed
                };

            }
                
            else
                progresses[q.questID] = new ProgressData(q.questID);
        }

        dailyWinCount = data.dailyWinCount;

        dailyQuestChestClaimed = data.dailyQuestChestClaimed;
        dailyWinChestClaimed = data.dailyWinChestClaimed;
    }


    public ProgressData GetProgress(string id)
    {
        if (!progresses.TryGetValue(id, out var p))
        {
            p = new ProgressData(id);
            progresses[id] = p;
        }
        return p;
    }

    public int GetClaimedQuestCount()
    {
        return progresses.Values.Count(p => p.isClaimed);
    }

    public int GetTotalDailyQuest()
    {
        return dailyQuestsData.Count;
    }

    public float GetDailyQuestProgress() => (float)GetClaimedQuestCount() / GetTotalDailyQuest();
}
