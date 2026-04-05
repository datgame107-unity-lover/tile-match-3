// Scripts/UI/WarningUIPanel.cs
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningConfig
{
    public string message;
    public string confirmLabel;
    public string cancelLabel;
    public System.Action onConfirm;
    public System.Action onCancel;
}

public class WarningUIPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popup;

    [Header("Content")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private TextMeshProUGUI cancelText;

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Image confirmBg;      // set color #B8F0DC (mint green)
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image cancelBg;       // set color #2C2C2A (dark)

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float popDuration = 0.3f;

    // màu cố định theo design
    private static readonly Color ConfirmBgColor = new Color(0.722f, 0.941f, 0.863f); // #B8F0DC
    private static readonly Color ConfirmTxtColor = new Color(0.031f, 0.314f, 0.255f); // #085041
    private static readonly Color CancelBgColor = new Color(0.173f, 0.173f, 0.165f); // #2C2C2A
    private static readonly Color CancelTxtColor = new Color(0.945f, 0.937f, 0.910f); // #F1EFE8

    private WarningConfig _config;

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        ApplyButtonColors();

        confirmButton?.onClick.AddListener(OnConfirmClicked);
        cancelButton?.onClick.AddListener(OnCancelClicked);
        EventBus<ShowWarningEvent>.Subscribe(OnShowWarning);

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        confirmButton?.onClick.RemoveListener(OnConfirmClicked);
        cancelButton?.onClick.RemoveListener(OnCancelClicked);
        EventBus<ShowWarningEvent>.Unsubscribe(OnShowWarning);
    }

    // ── Colors ───────────────────────────────────────

    private void ApplyButtonColors()
    {
        if (confirmBg) confirmBg.color = ConfirmBgColor;
        if (confirmText) confirmText.color = ConfirmTxtColor;
        if (cancelBg) cancelBg.color = CancelBgColor;
        if (cancelText) cancelText.color = CancelTxtColor;
    }

    // ── Handler ──────────────────────────────────────

    private void OnShowWarning(ShowWarningEvent evt) => Show(evt.config);

    // ── Show ─────────────────────────────────────────

    private void Show(WarningConfig config)
    {
        _config = config;

        if (messageText) messageText.text = config.message ?? "";
        if (confirmText) confirmText.text = config.confirmLabel ?? "Yes";
        if (cancelText) cancelText.text = config.cancelLabel ?? "Return";

        var seq = DOTween.Sequence();

        if (canvasGroup)
        {
            seq.AppendCallback(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
            seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        }

        if (popup)
        {
            popup.localScale = Vector3.zero;
            seq.Append(popup
                .DOScale(Vector3.one, popDuration)
                .SetEase(Ease.OutBack));
        }

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

    private void OnConfirmClicked()
    {
        Hide();
        _config?.onConfirm?.Invoke();
        _config = null;
    }

    private void OnCancelClicked()
    {
        Hide();
        _config?.onCancel?.Invoke();
        _config = null;
    }
}