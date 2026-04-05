// Scripts/UI/ToggleSwitch.cs
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MaximovInk.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform knob;
    [SerializeField] private RoundedPanel background;

    [Header("Colors")]
    [SerializeField] private Color colorOn = new Color(0.498f, 0.467f, 0.925f); // #7F77DD
    [SerializeField] private Color colorOff = new Color(0.706f, 0.663f, 0.925f); // #AFA9EC

    [Header("Settings")]
    [SerializeField] private bool isOn = true;
    [SerializeField] private float knobOffsetX = 20f;  // khoảng dịch knob (on - off)
    [SerializeField] private float animDuration = 0.25f;

    public bool IsOn => isOn;
    public event Action<bool> OnValueChanged;

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        // set trạng thái ban đầu không animate
        ApplyState(animate: false);
    }

    // ── Click ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        SetValue(!isOn);
    }

    // ── Public API ────────────────────────────────────

    public void SetValue(bool value, bool animate = true)
    {
        if (isOn == value) return;
        isOn = value;
        ApplyState(animate);
        OnValueChanged?.Invoke(isOn);
    }

    // ── Private ───────────────────────────────────────

    private void ApplyState(bool animate)
    {
        float targetX = isOn ? knobOffsetX : -knobOffsetX;
        Color targetColor = isOn ? colorOn : colorOff;

        if (animate)
        {
            knob.DOAnchorPosX(targetX, animDuration).SetEase(Ease.OutBack);
            background.DOColor(targetColor, animDuration);
        }
        else
        {
            var pos = knob.anchoredPosition;
            pos.x = targetX;
            knob.anchoredPosition = pos;
            background.color = targetColor;
        }
    }
}