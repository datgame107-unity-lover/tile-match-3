// Scripts/Gameplay/Tile/TileInputSystem.cs
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TileInputSystem
{
    private readonly BoardContext context;
    private Tile pressedTile;

    public TileInputSystem(BoardContext context)
    {
        this.context = context;
        EnhancedTouchSupport.Enable();
    }

    public void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    // ── Mouse ─────────────────────────────────────────
    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        var tile = GetTopTileUnderPointer(mouse.position.ReadValue());

        if (mouse.leftButton.wasPressedThisFrame) Press(tile);
        if (mouse.leftButton.wasReleasedThisFrame) Release(tile);
    }

    // ── Touch ─────────────────────────────────────────
    private void HandleTouchInput()
    {
        if (Touch.activeTouches.Count == 0) return;

        var touch = Touch.activeTouches[0];
        var tile = GetTopTileUnderPointer(touch.screenPosition);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                Press(tile);
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                Release(tile);
                break;
        }
    }

    // ── Press / Release ───────────────────────────────
    private void Press(Tile tile)
    {
        if (tile == null || tile.isBlocked) return;

        pressedTile = tile;

        var c = tile.transform.Find("Container");
        if (c == null) return;

        c.DOKill();
        //DOAnimationManager.ScaleBounce(
        //    c, tile.GetOriginalScale(), 1.2f, 0.2f, false);
    }

    private void Release(Tile tileUnderPointer)
    {
        if (pressedTile == null) return;

        bool sameTile = tileUnderPointer == pressedTile;
        var c = pressedTile.transform.Find("Container");

        c?.DOKill();

        if (sameTile)
        {
            if (!pressedTile.isClicked && !pressedTile.isBlocked)
            {
                context.Manager.SelectTile(pressedTile); 
            }
        }
        else
        {
            if (c != null)
            {
                //DOAnimationManager.ScaleBounce(
                //   c, pressedTile.GetOriginalScale(), 1f, 0.2f);
            }
               
        }

        pressedTile = null;
    }

    // ── Raycast ───────────────────────────────────────
    private Tile GetTopTileUnderPointer(Vector2 screenPos)
    {
        var cam = Camera.main;
        var world = cam.ScreenToWorldPoint(new Vector3(
            screenPos.x, screenPos.y,
            Mathf.Abs(cam.transform.position.z)));

        var hits = Physics2D.RaycastAll(world, Vector2.zero);
        int highestLayer = int.MinValue;
        Tile topTile = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            var t = hit.collider.GetComponent<Tile>();
            if (t == null) continue;
            if (t.layer >= highestLayer)
            {
                highestLayer = t.layer;
                topTile = t;
            }
        }

        return topTile;
    }
}