using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public GameObject achivementItemPrefabs;
    private AchievementManager manager;

    private void Start()
    {   
        manager = AchievementManager.Instance;
        LoadAchievement();
    }

    private void LoadAchievement()
    {
        foreach (AchievementData data in manager.achivementDatas)
        {
            AchivementItemUI ui = Instantiate(achivementItemPrefabs, transform.Find("AchivementScroll/Viewport/Content")).GetComponent<AchivementItemUI>();
            ui.SetUp(data);

        }
    }

}
