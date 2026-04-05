// Scripts/UI/Quest/QuestItemUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemUI : MonoBehaviour
{
    [Header("Refs")]
    public Image tileIcon;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI progressText;
    public Image progressFill;
    public Button claimButton;
    public Image rewardIcon;
    public GameObject completedOverlay;

    private QuestDataSO _data;
    private ProgressService _progress;

    public void Init(QuestDataSO data, ProgressService progress)
    {
        _data = data;
        _progress = progress;

        questName.text = data.questName;

        if (data.targetTile != null && tileIcon != null)
            tileIcon.sprite = data.targetTile.sprite;

        claimButton.onClick.AddListener(OnClaim);
        Refresh();
    }

    public void Refresh()
    {
        var p = _progress.GetQuestProgress(_data.questID);
        int current = p.currentAmount;
        int target = _data.targetAmount;

        progressText.text = $"{current}/{target}";
        // progress bar
        if (progressFill != null)
            progressFill.fillAmount = (float)current / target;


        bool completed = current >= target;
        bool claimed = p.isClaimed;

        claimButton.gameObject.SetActive(completed && !claimed);

        if (completedOverlay != null)
            completedOverlay.SetActive(claimed);
    }

    private void OnClaim()
    {
        _progress.ClaimQuest(_data.questID);
        Refresh();
    }
}