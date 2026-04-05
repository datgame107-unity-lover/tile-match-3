// Scripts/UI/AchievementItemUI.cs
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItemUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;  // "3/5"
    [SerializeField] private Image progressFill;
    [SerializeField] private GameObject claimedBadge;  // dấu tick / "Claimed"
    [SerializeField] private Button button;

    private AchievementDataSO _data;
    private Action<AchievementDataSO> _onClick;

    public void Bind(
        AchievementDataSO data,
        AchievementProgress progress,
        Action<AchievementDataSO> onClick)
    {
        _data = data;
        _onClick = onClick;

        if (iconImage) iconImage.sprite = data.icon;
        if (titleText) titleText.text = data.title;

        float percent = Mathf.Clamp01((float)progress.current / data.target);

        if (progressText)
            progressText.text = $"{progress.current}/{data.target}";

        if (progressFill)
        {
            progressFill.fillAmount = 0f;
            progressFill.DOFillAmount(percent, 0.4f).SetEase(Ease.OutCubic);
        }

        if (claimedBadge) claimedBadge.SetActive(progress.isClaimed);

        button?.onClick.AddListener(() => _onClick?.Invoke(_data));
    }
}