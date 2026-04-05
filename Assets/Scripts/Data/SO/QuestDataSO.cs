// Scripts/Data/SO/QuestDataSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Game/Quest Data")]
public class QuestDataSO : ScriptableObject
{
    public string questID;
    public string questName;
    public Sprite icon;
    public QuestType type;
    public TileDataSO targetTile;   // chỉ dùng khi type = CollectItem
    public int targetAmount;
    public List<QuestReward> rewards;
}