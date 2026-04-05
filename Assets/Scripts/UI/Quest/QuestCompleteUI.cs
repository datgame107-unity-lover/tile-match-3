using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompleteUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform panel;

    [Header("Animation")]
    [SerializeField] private float offsetY = 400f;
    [SerializeField] private float showDuration = 0.45f;
    [SerializeField] private float hideDuration = 0.3f;
    [SerializeField] private float visibleTime = 2.5f;

    [Header("Quest info")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image progress;
    [SerializeField] private Image finishedIcon;
    private readonly Queue<QuestDataSO> queue = new();
    private bool playing;
    private Vector2 hiddenPos;
    private Vector2 shownPos;

    private Tween currentTween;

    // =====================================================
    // INIT
    // =====================================================
    private void Awake()
    {
        shownPos = panel.anchoredPosition;
        hiddenPos = shownPos + Vector2.up * offsetY;

        // bắt đầu ở ngoài màn hình
        panel.anchoredPosition = hiddenPos;
    }

    // =====================================================
    // EVENT SUBSCRIBE
    // =====================================================
    private void OnEnable()
    {
        EventBus<QuestCompletedEvent>.Subscribe(OnQuestCompleted);
    }

    private void OnDisable()
    {
        EventBus<QuestCompletedEvent>.Unsubscribe(OnQuestCompleted);
    }

    // =====================================================
    // EVENT HANDLER
    // =====================================================
    private void OnQuestCompleted(QuestCompletedEvent evt)
    {
        queue.Enqueue(evt.quest);

        if (!playing)
            PlayNext();
    }
    private void PlayNext()
    {
        if (queue.Count == 0)
        {
            playing = false;
            return;
        }

        playing = true;
        var quest = queue.Dequeue();

        currentTween?.Kill();

        // =========================
        // SET DATA UI
        // =========================
        icon.sprite = quest.icon;
        questName.text = quest.questName;

        progress.fillAmount = 0f;
        finishedIcon.gameObject.SetActive(false);
        finishedIcon.transform.localScale = Vector3.zero;
        finishedIcon.transform.localRotation = Quaternion.identity;

        // =========================
        // SEQUENCE
        // =========================
        Sequence seq = DOTween.Sequence();

        // ===== PANEL DROP FROM TOP =====
        seq.Append(
            panel.DOAnchorPos(shownPos, showDuration)
                .SetEase(Ease.OutBack)
        );

        // ===== PROGRESS FILL =====
        seq.Append(
            progress.DOFillAmount(1f, 0.6f)
                .SetEase(Ease.OutCubic)
        );

        // ===== SHOW FINISHED ICON =====
        seq.AppendCallback(() =>
        {
            finishedIcon.gameObject.SetActive(true);
        });

        // Scale in
        seq.Append(
            finishedIcon.transform
                .DOScale(1.2f, 0.25f)
                .SetEase(Ease.OutBack)
        );

        // Rotate 1 vòng + punch scale
        seq.Join(
            finishedIcon.transform
                .DORotate(new Vector3(0, 0, 360f), 0.45f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
        );

        seq.Append(
            finishedIcon.transform
                .DOScale(1f, 0.15f)
                .SetEase(Ease.InOutSine)
        );

        // ===== WAIT =====
        seq.AppendInterval(visibleTime);

        // ===== HIDE =====
        seq.Append(
            panel.DOAnchorPos(hiddenPos, hideDuration)
                .SetEase(Ease.InBack)
        );

        seq.OnComplete(PlayNext);

        currentTween = seq;
    }
    // =====================================================
    // SHOW / HIDE
    // =====================================================

}