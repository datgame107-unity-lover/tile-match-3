using UnityEngine;

public enum QuestType
{
    CollectItem,
    UseAbility,
    CompleteLevels,
    EarnCoins,
    StrikeCombo,
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest Data")]
public class QuestDataSO : ScriptableObject
{
    public string questID;
    public Sprite icon;
    public string description;
    public QuestType type;

    [Header("Target")]
    public int targetAmount;

    [Header("Specific Target (Optional)")]
    public TileDataSO targetTile;

    [Header("Rewards")]
    public QuestReward[] rewards;
}

[System.Serializable]
public class QuestReward
{
    public CurrencyType type;
    public int amount;
    public Sprite icon;
}
