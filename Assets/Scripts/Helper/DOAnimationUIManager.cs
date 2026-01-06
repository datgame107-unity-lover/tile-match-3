using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public static class DOAnimationManager
{
    // Scale animation sử dụng chung
    public static void ScaleBounce(Transform target, Vector3 originalScale, float scaleMultiplier = 1.2f, float duration = 0.1f)
    {
        if (target == null) return;

        target.DOKill(true);

        target.localScale = originalScale;

        target
            .DOScale(originalScale * scaleMultiplier, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                target
                    .DOScale(originalScale, duration)
                    .SetEase(Ease.OutQuad);
            });
    }
    public static void ScaleBounce(Transform target, Vector3 originalScale, float scaleMultiplier = 1.2f, float duration = 0.1f, bool resetScale = true)
    {
        if (target == null) return;

        target.DOKill(true);

        target.localScale = originalScale;

        target
            .DOScale(originalScale * scaleMultiplier, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {   
                if(resetScale) 
                target
                    .DOScale(originalScale, duration)
                    .SetEase(Ease.OutQuad);
            });
    }
    public static void ScaleBounce(Transform target, float duration = 0.1f)
    {
        target.DOKill(true);

        if (target == null) return;

        Vector3 originalScale = target.localScale;
        target.DOScale(originalScale * 1.2f, duration)
            .SetEase(Ease.OutQuad);
            
    }
    public static void ImageDropDown(RectTransform target, float distance = 150f, float duration = 0.35f)
    {
        if (target == null) return;

        target.DOKill(true);

        Vector3 startPos = target.anchoredPosition;
        Vector3 fromPos = startPos + new Vector3(0, distance, 0);

        // Set vị trí bắt đầu trước khi animate
        target.anchoredPosition = fromPos;

        target
            .DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutBack); // rơi xuống rồi bật nhẹ
    }
    public static void Shake(Transform target, float strength = 15f, float duration = 0.5f)
    {
        if (target == null) return;

        target.DOKill(true);

        target.DOShakeRotation(
            duration,
            new Vector3(0, 0, strength),
            vibrato: 10,
            randomness: 90
        ).SetEase(Ease.OutQuad);
    }

    public static void TextBounce(Transform target, float scaleUp = 1.2f, float duration = 0.5f)
    {
        if (target == null) return;

        Vector3 original = target.localScale;
        target.DOKill(true);

        target.localScale = original;

        target
            .DOScale(original * scaleUp, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                target.DOScale(original, duration).SetEase(Ease.OutQuad);
            });
    }
    public static void MoveFillBar(Image fillBar, float targetFill, float duration = 1f)
    {
        if (fillBar == null) return;

        // Kill nhưng KHÔNG complete (true = completeImmediately)
        fillBar.DOKill(false);

        float startFill = fillBar.fillAmount;

        // Bảo đảm tween chạy mượt, không giật frame đầu
        fillBar.DOFillAmount(targetFill, duration)
               .SetEase(Ease.OutQuad)
               .From(startFill);  // khóa giá trị bắt đầu
    }


    public static void ShakeOnce(Transform target, float strength = 20f, float duration = 0.25f)
    {
        if (target == null) return;

        target.DOKill(true);

        target.DOShakeRotation(
            duration,
            new Vector3(0, 0, strength),
            vibrato: 10,
            randomness: 90
        ).SetEase(Ease.OutQuad);
    }

    public static void ShakeLeftRight(Transform target, float distance = 20f, float duration = 0.3f, int loops = 1)
    {
        if (target == null) return;

        target.DOKill(true);

        float half = duration / 2f;

        Sequence seq = DOTween.Sequence();

        seq.Append(target.DOLocalMoveX(target.localPosition.x - distance, half).SetEase(Ease.OutQuad))
           .Append(target.DOLocalMoveX(target.localPosition.x + distance, half).SetEase(Ease.OutQuad))
           .SetLoops(loops, LoopType.Restart)
           .OnComplete(() =>
           {
               // Reset to original position
               target.DOLocalMoveX(target.localPosition.x, 0.05f);
           });
    }


}
