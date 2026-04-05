// Scripts/Data/SO/AdsRewardDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "AdsRewardData", menuName = "Game/Ads Reward Data")]
public class AdsRewardDataSO : ScriptableObject
{
    public CurrencyType rewardType;
    public int rewardAmount;
}