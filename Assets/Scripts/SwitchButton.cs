using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SwitchButton : MonoBehaviour, IPointerClickHandler
{
    public Slider slider;
    public Image fillImage;

    [Header("Colors")]
    public Color onColor = new Color(0.6f, 0.95f, 0.6f);   // xanh cỏ bật
    public Color offColor = new Color(0.8f, 0.8f, 0.8f);   // xám tắt

    [Header("Animation")]
    public float duration = 0.25f;     // Thời gian animation
    public float cooldown = 0.3f;      // Giới hạn đổi trạng thái

    private bool isOn;
    private bool canToggle = true;
    private Tween cooldownTween;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.wholeNumbers = true;


        isOn = slider.value > 0.5f;
        UpdateSwitch(false);

        slider.onValueChanged.RemoveAllListeners();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canToggle) return;

        canToggle = false;
        isOn = !isOn; // Đảo trạng thái
        UpdateSwitch(true);

        // Reset cooldown sau 0.5 giây
        cooldownTween?.Kill();
        cooldownTween = DOVirtual.DelayedCall(cooldown, () => canToggle = true);
    }

    private void UpdateSwitch(bool animate)
    {
        Color targetColor = isOn ? onColor : offColor;

        if (animate)
        {
            fillImage.DOColor(targetColor, duration);
        }
        else
        {
            fillImage.color = targetColor;
        }

        slider.DOValue(isOn ? 1 : 0, animate ? duration : 0f).SetEase(Ease.OutQuad);

        Debug.Log("Switch: " + (isOn ? "ON" : "OFF"));
    }
}
