// Scripts/UI/ComboUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Hiển thị combo count và thanh đếm ngược thời gian combo.
/// Lắng nghe ComboChangedEvent và ComboResetEvent từ EndlessModeHandler.
///
/// Hierarchy gợi ý:
///   ComboUI (GameObject)
///   ├── ComboGroup (CanvasGroup – để fade in/out)
///   │   ├── ComboLabel  (TMP_Text – "COMBO")
///   │   ├── ComboCount  (TMP_Text – "×7")
///   │   └── ComboTimer  (Image – fillAmount, Fill Horizontal)
/// </summary>
public class ComboUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text comboCountText;   // hiển thị "×7"
    [SerializeField] private TMP_Text comboLabelText;   // hiển thị "COMBO"
    [SerializeField] private Image comboTimerImage;  // fill bar đếm ngược
    [SerializeField] private CanvasGroup canvasGroup;   // fade toàn bộ

    [Header("Màu chữ theo milestone")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorSpecial2 = new Color(1f, 0.85f, 0.2f);  // combo 2 (same type)
    [SerializeField] private Color colorSpecial3 = new Color(1f, 0.45f, 0.2f);  // combo 3 (AOE)
    [SerializeField] private Color colorSpecial7 = new Color(1f, 0.2f, 0.6f);  // combo 7 (clear all)

    [Header("Animation")]
    [SerializeField] private float punchScale = 0.25f;
    [SerializeField] private float punchDuration = 0.25f;
    [SerializeField] private float fadeDuration = 0.3f;

    private Tweener timerTween;

    private void OnEnable()
    {
        EventBus<ComboChangedEvent>.Subscribe(OnComboChanged);
        EventBus<ComboResetEvent>.Subscribe(OnComboReset);
    }

    private void OnDisable()
    {
        EventBus<ComboChangedEvent>.Unsubscribe(OnComboChanged);
        EventBus<ComboResetEvent>.Unsubscribe(OnComboReset);
        timerTween?.Kill();
    }

    private void Start()
    {
        SetVisible(false, instant: true);
    }

    // ── Event handlers ────────────────────────────────

    private void OnComboChanged(ComboChangedEvent e)
    {
        // Hiện panel nếu đang ẩn
        if (canvasGroup != null && canvasGroup.alpha < 0.1f)
            SetVisible(true);

        // Cập nhật số combo
        if (comboCountText != null)
        {
            comboCountText.text = $"×{e.count}";
            comboCountText.color = GetComboColor(e.count);

            // Punch scale mỗi khi tăng (timeRatio == 1 nghĩa là vừa match)
            if (Mathf.Approximately(e.timeRatio, 1f))
                PlayPunch();
        }

        // Cập nhật timer fill
        if (comboTimerImage != null)
        {
            timerTween?.Kill();
            timerTween = comboTimerImage
                .DOFillAmount(e.timeRatio, 0.05f)
                .SetEase(Ease.Linear);

            comboTimerImage.color = e.timeRatio > 0.3f
                ? colorNormal
                : new Color(1f, 0.3f, 0.3f); // đỏ khi gần hết
        }
    }

    private void OnComboReset(ComboResetEvent e)
    {
        SetVisible(false);
    }

    // ── Helpers ───────────────────────────────────────

    private Color GetComboColor(int count) => count switch
    {
        >= 7 => colorSpecial7,
        3 => colorSpecial3,
        2 => colorSpecial2,
        _ => colorNormal
    };

    private void PlayPunch()
    {
        comboCountText.transform
            .DOPunchScale(Vector3.one * punchScale, punchDuration, 5, 0.5f)
            .SetEase(Ease.OutElastic);
    }

    private void SetVisible(bool visible, bool instant = false)
    {
        if (canvasGroup == null) return;

        canvasGroup.DOKill();
        float target = visible ? 1f : 0f;

        if (instant)
        {
            canvasGroup.alpha = target;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        else
        {
            canvasGroup.DOFade(target, fadeDuration)
                .OnComplete(() =>
                {
                    canvasGroup.interactable = visible;
                    canvasGroup.blocksRaycasts = visible;
                });
        }
    }
}