// Scripts/Data/SO/AchievementDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "Game/Achievement Data")]
public class AchievementDataSO : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    public AchievementType type;
    public int target;
    public CurrencyType rewardType;
    public int rewardAmount;
    public Sprite icon;
}