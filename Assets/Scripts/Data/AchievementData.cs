using System.Collections.Generic;
using UnityEngine;

public enum AchievementType
{
    PlayGame,
    ClearLevel,          
    CollectItem,        
    MatchTiles,         
    UseBooster,
    HighScore,
    

    // Currency-based
    EarnFlower,
    SpendFlower,
    EarnDiamond,
    SpendDiamond,
    LoseHeart,
    // Special
    LoginDays,           // Login liên tiếp
    CompleteQuest,
}

[CreateAssetMenu(
    fileName = "AchievementData",
    menuName = "Game/Achievement/Achievement Data"
)]
public class AchievementData : ScriptableObject
{
    public AchievementType type;
    [Header("Achivement ")]
    public string id;
    public string name;
    public Sprite icon;
    public string description;

    [Header("Progress")]
    public int target;

    [Header("Reward")]
    public List<QuestReward> rewards;


    [Header("State")]
    public bool disabled = false;

    private void OnValidate()
    {
        if(rewards.Count>2)
            rewards.RemoveAt(rewards.Count-1);
    }
}
