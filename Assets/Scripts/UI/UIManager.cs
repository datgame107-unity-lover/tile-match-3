// Scripts/UI/UIManager.cs
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public QuestPanelUI questPanel;
    public HomePanelUI homePanel;
    public AchievementPanelUI achievementPanel;
    private void Start()
    {   
        var progress = ServiceLocator.Get<ProgressService>();
        print(progress);
        var currency = ServiceLocator.Get<CurrencyService>();
        var save = ServiceLocator.Get<ISaveService>();
        var leaderBoard = ServiceLocator.Get<ILeaderboardService>();
        var level = ServiceLocator.Get<LevelJsonService>();
        questPanel.Init(progress, currency );
        homePanel.Init(save, currency, leaderBoard, level);
        achievementPanel.Init(progress);    

    }
}