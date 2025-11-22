using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SwitchButton : MonoBehaviour, IPointerClickHandler
{
    public Slider slider;
    public Image fillImage;

    [Header("Colors")]
    public Color onColor = new Color(0.6f, 0.95f, 0.6f);
    public Color offColor = new Color(0.8f, 0.8f, 0.8f);

    [Header("Animation")]
    public float duration = 0.25f;
    public float cooldown = 0.3f;

    private bool isOn;
    private bool canToggle = true;
    private Tween cooldownTween;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.wholeNumbers = true;

        isOn = slider.value > 0.5f;
        UpdateVisual(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canToggle) return;

        canToggle = false;
        isOn = !isOn;

        // Gọi animation
        UpdateVisual(true);

        // ⚡ Gọi thủ công sự kiện cho SettingUI
        slider.SetValueWithoutNotify(isOn ? 1 : 0); // đổi giá trị mà không loop listener
        slider.onValueChanged.Invoke(isOn ? 1 : 0); // ép Unity chạy listener của SettingUI

        cooldownTween?.Kill();
        cooldownTween = DOVirtual.DelayedCall(cooldown, () => canToggle = true);
    }

    private void UpdateVisual(bool animate)
    {
        Color targetColor = isOn ? onColor : offColor;

        if (animate)
            fillImage.DOColor(targetColor, duration);
        else
            fillImage.color = targetColor;

        slider.DOValue(isOn ? 1 : 0, animate ? duration : 0f).SetEase(Ease.OutQuad);
    }
}
