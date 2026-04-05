// Scripts/UI/AchievementInfoView.cs
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInfoView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popup;

    [Header("Content")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image progressFill;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button claimButton;
    [SerializeField] private TextMeshProUGUI claimButtonText;

    [Header("Backdrop")]
    [SerializeField] private Button backdropButton;

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float popDuration = 0.3f;

    private AchievementDataSO _current;
    private ProgressService _progress;

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        backdropButton?.onClick.AddListener(Hide);
        claimButton?.onClick.AddListener(OnClaimClicked);
    }

    private void Start()
    {
        _progress = ServiceLocator.Get<ProgressService>();
    }

    private void OnDestroy()
    {
        backdropButton?.onClick.RemoveListener(Hide);
        claimButton?.onClick.RemoveListener(OnClaimClicked);
    }

    // ── Public API ────────────────────────────────────

    public void Show(AchievementDataSO data, AchievementProgress progress)
    {
        _current = data;

        // bind content
        if (iconImage) iconImage.sprite = data.icon;
        if (titleText) titleText.text = data.title;
        if (descText) descText.text = data.description;

        float percent = Mathf.Clamp01((float)progress.current / data.target);
        if (progressFill) progressFill.fillAmount = percent;
        if (progressText) progressText.text = $"{progress.current}/{data.target}";

        // claim button state
        bool canClaim = progress.current >= data.target && !progress.isClaimed;
        if (claimButton) claimButton.interactable = canClaim;
        if (claimButtonText) claimButtonText.text = progress.isClaimed ? "Claimed" : "Claim";

        // animate show
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        var seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        if (popup)
        {
            popup.localScale = Vector3.zero;
            seq.Append(popup.DOScale(Vector3.one, popDuration).SetEase(Ease.OutBack));
        }
        seq.Play();
    }

    public void Hide()
    {
        if (canvasGroup == null) return;
        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    // ── Claim ─────────────────────────────────────────

    private void OnClaimClicked()
    {
        if (_current == null) return;
        _progress.ClaimAchievement(_current.id);
        Hide();
    }
}