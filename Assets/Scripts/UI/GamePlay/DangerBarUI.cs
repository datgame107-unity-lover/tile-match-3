// Scripts/UI/DangerBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Hiển thị thanh nguy hiểm (Danger Bar) dùng Image.fillAmount.
/// Lắng nghe DangerSystem qua EventBus<DangerChangedEvent>.
/// Gắn vào GameObject có Image component (Fill Method = Horizontal).
/// </summary>
public class DangerBarUI : MonoBehaviour
{
    [Header("Fill Image")]
    [SerializeField] private Image fillImage;

    [Header("Gradient màu theo mức nguy hiểm")]
    [SerializeField] private Color colorSafe = new Color(0.31f, 0.75f, 0.38f); // xanh lá
    [SerializeField] private Color colorCaution = new Color(0.94f, 0.75f, 0.13f); // vàng
    [SerializeField] private Color colorDanger = new Color(0.91f, 0.25f, 0.31f); // đỏ
    [SerializeField] private Color colorCritical = new Color(0.75f, 0.06f, 0.19f); // đỏ đậm

    [Header("Ngưỡng (0-1)")]
    [SerializeField] private float thresholdCaution = 0.35f;
    [SerializeField] private float thresholdDanger = 0.60f;
    [SerializeField] private float thresholdCritical = 0.80f;

    [Header("Animation")]
    [SerializeField] private float tweenDuration = 0.25f;
    [SerializeField] private float flashInterval = 0.4f; // nhấp nháy khi critical

    private Tweener fillTween;
    private Tweener flashTween;
    private bool isCritical;

    private void Awake()
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        EventBus<DangerChangedEvent>.Subscribe(OnDangerChanged);
    }

    private void OnDisable()
    {
        EventBus<DangerChangedEvent>.Unsubscribe(OnDangerChanged);
        fillTween?.Kill();
        flashTween?.Kill();
    }

    private void OnDangerChanged(DangerChangedEvent e)
    {
        SetFill(e.value);
    }

    /// <summary>
    /// Cập nhật UI từ bên ngoài (ratio: 0 = an toàn, 1 = nguy hiểm tối đa).
    /// </summary>
    public void SetFill(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);

        // Tween fill
        fillTween?.Kill();
        fillTween = fillImage.DOFillAmount(ratio, tweenDuration).SetEase(Ease.OutQuad);

        // Màu sắc
        Color targetColor = GetColorForRatio(ratio);
        fillImage.DOColor(targetColor, tweenDuration);

        // Flash khi critical
        bool nowCritical = ratio >= thresholdCritical;
        if (nowCritical && !isCritical)
            StartFlash();
        else if (!nowCritical && isCritical)
            StopFlash();

        isCritical = nowCritical;
    }

    private Color GetColorForRatio(float ratio)
    {
        if (ratio >= thresholdCritical)
            return colorCritical;
        if (ratio >= thresholdDanger)
            return Color.Lerp(colorCaution, colorDanger,
                (ratio - thresholdDanger) / (thresholdCritical - thresholdDanger));
        if (ratio >= thresholdCaution)
            return Color.Lerp(colorSafe, colorCaution,
                (ratio - thresholdCaution) / (thresholdDanger - thresholdCaution));
        return colorSafe;
    }

    private void StartFlash()
    {
        flashTween?.Kill();
        flashTween = fillImage
            .DOFade(0.4f, flashInterval)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopFlash()
    {
        flashTween?.Kill();
        fillImage.DOFade(1f, 0.15f);
    }
}