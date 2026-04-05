// Scripts/UI/Achievement/AchievementPanelUI.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPanelUI : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI totalText;       // "18/41"
    [SerializeField] private TextMeshProUGUI claimedText;     // "3 Claimed"

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI completedText;
    [SerializeField] private TextMeshProUGUI claimedCountText;
    [SerializeField] private TextMeshProUGUI inProgressText;

    [Header("List")]
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject itemPrefab;

    [Header("Info View")]
    [SerializeField] private AchievementInfoView infoView;

    private ProgressService _progress;
    private List<AchievementDataSO> _achievements;

    private void OnEnable() => Refresh();
    private void OnDestroy() => UnsubscribeEvents();

    // ── Init (gọi từ UIManager giống HomePanelUI) ────────

    public void Init(ProgressService progress)
    {
        _progress = progress;
        _achievements = _progress.GetAchievements();

        SubscribeEvents();
        Refresh();
    }

    // ── Refresh ───────────────────────────────────────

    private void Refresh()
    {
        if (_progress == null || _achievements == null) return;

        int total = _achievements.Count;
        int completed = 0;
        int claimed = 0;

        foreach (var data in _achievements)
        {
            var p = _progress.GetAchievementProgress(data.id);
            if (p.current >= data.target) completed++;
            if (p.isClaimed) claimed++;
        }

        if (totalText) totalText.text = $"{completed}/{total}";
        if (claimedText) claimedText.text = $"{claimed} Claimed";
        if (completedText) completedText.text = completed.ToString();
        if (claimedCountText) claimedCountText.text = claimed.ToString();
        if (inProgressText) inProgressText.text = (total - completed).ToString();

        BuildList();
    }

    private void BuildList()
    {
        if (listContainer == null || itemPrefab == null) return;

        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        foreach (var data in _achievements)
        {
            var go = Instantiate(itemPrefab, listContainer);
            var item = go.GetComponent<AchievementItemUI>();
            if (item == null) continue;

            var progress = _progress.GetAchievementProgress(data.id);
            item.Bind(data, progress, OnItemClicked);
        }
    }

    // ── Handlers ─────────────────────────────────────

    private void OnItemClicked(AchievementDataSO data)
    {
        var progress = _progress.GetAchievementProgress(data.id);
        infoView?.Show(data, progress);
    }

    // ── Events ────────────────────────────────────────

    private void SubscribeEvents()
    {
        EventBus<AchievementUnlockedEvent>.Subscribe(OnAchievementUnlocked);
    }

    private void UnsubscribeEvents()
    {
        EventBus<AchievementUnlockedEvent>.Unsubscribe(OnAchievementUnlocked);
    }

    private void OnAchievementUnlocked(AchievementUnlockedEvent _) => Refresh();
}