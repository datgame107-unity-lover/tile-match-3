// Scripts/UI/WinUIPanel.cs
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUIPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;

    [Header("Rewards")]
    [SerializeField] private TextMeshProUGUI flowerCountText;
    [SerializeField] private TextMeshProUGUI diamondCountText;
    [SerializeField] private RectTransform flowerIcon;
    [SerializeField] private RectTransform diamondIcon;

    [Header("Daily Wins")]
    [SerializeField] private Image dailyWinsImage;

    [Header("Buttons")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float punchDuration = 0.4f;
    [SerializeField] private float sliderDuration = 0.8f;
    [SerializeField] private Ease sliderEase = Ease.OutCubic;

    private ProgressService _progress;

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        _progress = ServiceLocator.Get<ProgressService>();

        // subscribe ngay trong Awake — object vẫn active lúc này
        EventBus<PlayerWonEvent>.Subscribe(OnWin);
        homeButton?.onClick.AddListener(OnHomeClicked);
        continueButton?.onClick.AddListener(OnContinueClicked);

        // ẩn panel sau khi đã subscribe
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        EventBus<PlayerWonEvent>.Unsubscribe(OnWin);
        homeButton?.onClick.RemoveListener(OnHomeClicked);
        continueButton?.onClick.RemoveListener(OnContinueClicked);
    }

    // ── Handler ──────────────────────────────────────

    private void OnWin(PlayerWonEvent evt)
    {
        if (flowerCountText) flowerCountText.text = evt.flowersEarned.ToString();
        if (diamondCountText) diamondCountText.text = evt.diamondsEarned.ToString();
        if (dailyWinsImage) dailyWinsImage.fillAmount = 0f;

        PlayShowAnim();
    }

    // ── Anim ─────────────────────────────────────────

    private void PlayShowAnim()
    {
        var seq = DOTween.Sequence();

        // 1. fade in + bật tương tác
        if (canvasGroup)
        {
            seq.AppendCallback(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
            seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        }

        // 2. punch scale flower + diamond
        if (flowerIcon)
            seq.Append(flowerIcon.DOPunchScale(Vector3.one * 0.3f, punchDuration, 5, 0.5f));
        if (diamondIcon)
            seq.Join(diamondIcon.DOPunchScale(Vector3.one * 0.3f, punchDuration, 5, 0.5f));

        // 3. animate fillAmount — đọc progress sau khi sequence bắt đầu chạy
        //    lúc này ProgressService.OnPlayerWon() đã chạy xong
        if (dailyWinsImage)
            seq.AppendCallback(() =>
            {
                float targetFill = _progress.GetDailyWinProgress();
                seq.Append(
                    DOTween.To(
                        () => dailyWinsImage.fillAmount,
                        x => dailyWinsImage.fillAmount = x,
                        targetFill,
                        sliderDuration
                    ).SetEase(sliderEase)
                );
            });

        seq.Play();
    }
    // ── Hide ─────────────────────────────────────────

    private void Hide()
    {
        DOTween.Kill(canvasGroup);
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    // ── Buttons ───────────────────────────────────────

    private void OnHomeClicked()
    {
        SceneLoader.LoadHome();
    }

    private void OnContinueClicked()
    {
        Hide();
        EventBus<ContinueLevelEvent>.Publish(new ContinueLevelEvent());

    }
}