using UnityEngine;
using DG.Tweening;

public static class DOAnimationManager
{
    // Scale animation sử dụng chung
    public static void ScaleBounce(Transform target, float scaleMultiplier = 1.2f, float duration = 0.1f)
    {
        if (target == null) return;

        target.DOKill();
        Vector3 originalScale = target.localScale;
        target.DOScale(originalScale * scaleMultiplier, duration)
            .SetEase(Ease.OutQuad);
            //.OnComplete(() => target.DOScale(originalScale, duration).SetEase(Ease.OutQuad));
    }
   
}
