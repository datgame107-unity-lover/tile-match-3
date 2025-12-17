using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class RewardFlyUtil
{
    public static void Fly(
    Sprite icon,
    RectTransform from,
    RectTransform to,
    Canvas canvas,
    float duration = 1.2f)
    {
        if (icon == null) Debug.LogError("Fly: icon NULL");
        if (from == null) Debug.LogError("Fly: from NULL");
        if (to == null) Debug.LogError("Fly: target NULL");
        if (canvas == null) Debug.LogError("Fly: canvas NULL");

        if (icon == null || from == null || to == null || canvas == null)
            return;

        if (icon == null || from == null || to == null || canvas == null)
            return;

        GameObject go = new GameObject("RewardFly");
        go.transform.SetParent(canvas.transform, false);

        Image img = go.AddComponent<Image>();
        img.sprite = icon;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);          // SIZE CỐ ĐỊNH
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;

        CanvasGroup cg = go.AddComponent<CanvasGroup>();

        RectTransform canvasRT = canvas.transform as RectTransform;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT,
            RectTransformUtility.WorldToScreenPoint(cam, from.position),
            cam,
            out Vector2 start);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT,
            RectTransformUtility.WorldToScreenPoint(cam, to.position),
            cam,
            out Vector2 end);

        rt.anchoredPosition = start;

        float height = Mathf.Clamp(
            Mathf.Abs(start.x - end.x) * 0.3f,
            80f,
            160f
        );

        float highestY = Mathf.Max(start.y, end.y);

        Vector2 control = new Vector2(
            (start.x + end.x) * 0.5f,
            highestY + height
        );

        float t = 0f;

        DOTween.To(() => t, x =>
        {
            t = x;
            Vector2 pos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * control +
                Mathf.Pow(t, 2) * end;

            rt.anchoredPosition = pos;

        }, 1f, duration).SetEase(Ease.OutCubic);

        rt.DOScale(0.4f, duration);
        cg.DOFade(0f, duration).SetDelay(duration * 0.65f);

        DOVirtual.DelayedCall(duration, () =>
        {
            to.DOPunchScale(Vector3.one * 0.15f, 0.2f);
            Object.Destroy(go);
        });
    }
}
    