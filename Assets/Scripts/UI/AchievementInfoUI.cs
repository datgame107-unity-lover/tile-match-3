using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInfoUI : MonoBehaviour
{

    public Image achievementIcon;
    public TextMeshProUGUI achievementName;
    public TextMeshProUGUI achievementDescription;
    public TextMeshProUGUI achievementProgressText;
    public Image achievementProgress;
    public Button achievementClaimButton;

    public Sprite[] button;

    private AchievementManager manager;


    private void Start()
    {
        manager = AchievementManager.Instance;
    }
    public void LoadAchievement(AchievementData data)
    {
        achievementIcon.sprite = data.icon;
        achievementName.text = data.name;
        achievementDescription.text = data.description;
        achievementProgress.fillAmount = manager.GetProgressPercent(data.id);
        achievementProgressText.text = $"{manager.GetProgress(data.id).current}/{data.target}";
        achievementClaimButton.GetComponent<Image>().sprite =
    manager.GetProgress(data.id).isClaimed ? button[0] : button[1];


    }
}
