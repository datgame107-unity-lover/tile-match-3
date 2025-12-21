using UnityEngine;
using UnityEngine.UI;

public class AchivementItemUI : MonoBehaviour
{
    public Image achievementIcon;
    private AchievementData achievementData;
    public Button achievementButton;

    private AchievementInfoUI achievementInfoUI;


    private void Start()
    {
        achievementInfoUI = FindFirstObjectByType<AchievementInfoUI>();
    }
    private void OnEnable()
    {
        achievementButton.onClick.AddListener(() =>
        {
            achievementInfoUI.LoadAchievement(achievementData);

        });
    }
    public void SetUp(AchievementData data)
    {
        this.achievementData = data;
        achievementIcon.sprite = achievementData.icon;
    }
}
