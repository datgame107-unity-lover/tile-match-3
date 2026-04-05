// Scripts/UI/Shared/BottomNavBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BottomNavBarUI : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform panelContainer;
    public float screenWidth = 1080f;
    public float duration = 0.3f;
    public Ease ease = Ease.OutCubic;

    [Header("Nav Buttons")]
    public Button[] navButtons;
    public Color activeColor;
    public Color inactiveColor;

    public int CurrentIndex => _currentIndex;
    private int _currentIndex = 1;
    private Tweener _tween;

    private void Start()
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            int captured = i;
            navButtons[i].onClick.AddListener(() => OnNavButtonClick(captured));
        }

        // tạo tween 1 lần duy nhất, không auto kill
        _tween = panelContainer
            .DOAnchorPosX(0, duration)
            .SetEase(ease)
            .SetAutoKill(false)
            .Pause()
            .OnComplete(() => SetButtonsInteractable(true));

        JumpTo(_currentIndex);
    }

    private void OnNavButtonClick(int index)
    {
        if (index == _currentIndex) return;
        SetButtonsInteractable(false);
        AnimateTo(index);
    }

    public void ShowPanel(int index)
    {
        if (index == _currentIndex) return;
        SetButtonsInteractable(false);
        AnimateTo(index);
    }

    public void ShowPanelFromSwipe(int index)
    {
        _currentIndex = index;
        AnimateTo(index);
        UpdateNavButtons(index);
    }

    public void JumpTo(int index)
    {
        _tween.Pause();
        _currentIndex = index;
        var pos = panelContainer.anchoredPosition;
        pos.x = GetTargetX(index);
        panelContainer.anchoredPosition = pos;
        UpdateNavButtons(index);
        SetButtonsInteractable(true);
    }

    private void AnimateTo(int index)
    {
        _currentIndex = index;
        float targetX = GetTargetX(index);

        // thay đổi target và restart từ vị trí hiện tại
        _tween
            .ChangeStartValue(panelContainer.anchoredPosition)
            .ChangeEndValue(new Vector2(targetX, panelContainer.anchoredPosition.y))
            .Restart();

        UpdateNavButtons(index);
    }

    public float GetTargetX(int index) => (1 - index) * screenWidth;

    private void UpdateNavButtons(int index)
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            var img = navButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = i == index ? activeColor : inactiveColor;
        }
    }

    private void SetButtonsInteractable(bool value)
    {
        foreach (var btn in navButtons)
            btn.interactable = value;

        if (value && _currentIndex < navButtons.Length)
            navButtons[_currentIndex].interactable = false;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}