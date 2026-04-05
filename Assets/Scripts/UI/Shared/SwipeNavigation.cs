// Scripts/UI/Shared/SwipeNavigation.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

// Gán script này lên PanelContainer — không cần Image overlay
public class SwipeNavigation : MonoBehaviour
{
    public BottomNavBarUI navBar;
    public float swipeThreshold = 80f;
    public float decideDistance = 15f;

    private int _currentIndex = 1;
    private Vector2 _startPos;
    private bool _tracking = false;
    private bool _decided = false;
    private bool _isHorizontal = false;
    private float _containerStartX;

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    // ── Mouse (Editor) ───────────────────────────────────
    private void HandleMouse()
    {
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
            BeginTrack(mouse.position.ReadValue());
        else if (mouse.leftButton.isPressed && _tracking)
            MoveTrack(mouse.position.ReadValue());
        else if (mouse.leftButton.wasReleasedThisFrame && _tracking)
            EndTrack(mouse.position.ReadValue());
    }

    // ── Touch (Device) ───────────────────────────────────
    private void HandleTouch()
    {
        var touch = UnityEngine.InputSystem.Touchscreen.current;
        if (touch == null) return;

        var t = touch.primaryTouch;
        if (t.press.wasPressedThisFrame)
            BeginTrack(t.position.ReadValue());
        else if (t.press.isPressed && _tracking)
            MoveTrack(t.position.ReadValue());
        else if (t.press.wasReleasedThisFrame && _tracking)
            EndTrack(t.position.ReadValue());
    }

    // ── Track logic ──────────────────────────────────────
   private void BeginTrack(Vector2 pos)
{
    if (IsPointerOverInteractableUI(pos)) return;

    _startPos = pos;
    _tracking = true;
    _decided = false;
    _isHorizontal = false;
    // KHÔNG DOKill ở đây
    _containerStartX = navBar.panelContainer.anchoredPosition.x;
}

    private bool IsPointerOverInteractableUI(Vector2 screenPos)
    {
        var pe = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, results);
        foreach (var r in results)
        {
            var selectable = r.gameObject.GetComponentInParent<Selectable>();
            if (selectable != null && selectable.interactable)
                return true;
        }
        return false;
    }

    private void MoveTrack(Vector2 pos)
    {
        var delta = pos - _startPos;

        if (!_decided)
        {
            if (delta.magnitude < decideDistance) return;
            float angle = Mathf.Abs(Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            _isHorizontal = angle < 40f || angle > 140f;
            _decided = true;
        }

        if (!_isHorizontal) return;

        // check xem có bấm vào ScrollRect ngang không
        if (IsOverHorizontalScrollRect(pos)) return;

        float newX = _containerStartX + (pos.x - _startPos.x);
        float minX = (1 - 2) * navBar.screenWidth;
        float maxX = (1 - 0) * navBar.screenWidth;
        newX = Mathf.Clamp(newX, minX, maxX);

        var p = navBar.panelContainer.anchoredPosition;
        p.x = newX;
        navBar.panelContainer.anchoredPosition = p;
    }

    private void EndTrack(Vector2 pos)
    {
        _tracking = false;
        if (!_decided || !_isHorizontal) return;
        if (IsOverHorizontalScrollRect(pos)) return;

        float delta = pos.x - _startPos.x;
        if (Mathf.Abs(delta) >= swipeThreshold)
        {
            if (delta < 0 && _currentIndex < 2) _currentIndex++;
            else if (delta > 0 && _currentIndex > 0) _currentIndex--;
        }

        navBar.ShowPanelFromSwipe(_currentIndex);
    }

    // check pointer có đang trên ScrollRect ngang không
    private bool IsOverHorizontalScrollRect(Vector2 screenPos)
    {
        var pe = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, results);
        foreach (var r in results)
        {
            var sr = r.gameObject.GetComponentInParent<ScrollRect>();
            if (sr != null && sr.horizontal) return true;
        }
        return false;
    }
}