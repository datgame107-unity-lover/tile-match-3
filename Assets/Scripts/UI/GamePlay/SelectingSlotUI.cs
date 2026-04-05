using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectingSlotUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;

    [Header("Bounce (Show)")]
    [SerializeField] private float bounceDuration = 0.25f;
    [SerializeField] private Ease bounceEase = Ease.OutBack;

    [Header("Shrink (Clear)")]
    [SerializeField] private float shrinkDuration = 0.18f;
    [SerializeField] private Ease shrinkEase = Ease.InBack;

    public TileDataSO TileData { get; private set; }

    private Tweener _tween;

    public void Init(TileDataSO tileData)
    {
        TileData = tileData;

        if (iconImage == null) return;
        iconImage.sprite = tileData?.sprite;
        iconImage.enabled = iconImage.sprite != null;
    }

    public void Show()
    {
        _tween?.Kill();
        transform.localScale = Vector3.zero;
        _tween = transform
            .DOScale(Vector3.one, bounceDuration)
            .SetEase(bounceEase);
    }

    public void Clear()
    {
        _tween?.Kill();
        _tween = transform
            .DOScale(Vector3.zero, shrinkDuration)
            .SetEase(shrinkEase)
            .OnComplete(() => Destroy(gameObject));
    }
}