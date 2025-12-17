using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public QuestManager manager;
    public GameObject questItemPrefab;
    public Transform contentParent;
    public Image dailyQuestProgress;
    private void Start()
    {
        if(manager == null)
        {
            manager = QuestManager.Instance;
        }
    }

    private void OnEnable()
    {
        dailyQuestProgress.fillAmount = QuestManager.Instance.GetDailyQuestProgress();
        LoadQuests();
        EventManager.OnQuestClaimed += HandleQuestClaimed;
    }
    private void OnDisable()
    {
        EventManager.OnQuestClaimed -= HandleQuestClaimed;
    }

    private void HandleQuestClaimed(QuestDataSO quest)
    {
        float currentFill = dailyQuestProgress.fillAmount;

        float targetFill = Mathf.Clamp01(QuestManager.Instance.GetDailyQuestProgress());

        DOAnimationManager.MoveFillBar(dailyQuestProgress, targetFill, 0.6f);
    }
    public void LoadQuests()
    {
        // Xóa item cũ
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Tạo mới UI item
        foreach (var questData in manager.dailyQuestsData)
        {
            ProgressData progress = manager.GetProgress(questData.questID);

            QuestItemUI questItem = Instantiate(questItemPrefab, contentParent)
                                        .GetComponent<QuestItemUI>();

            questItem.Setup(questData);
        }
    }
}
