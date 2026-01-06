using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileUI : MonoBehaviour
{
    [Header("UI")]
    public SpriteRenderer food;
    public SpriteRenderer shadow;
    public Outline outline;

    [Header("Shadow")]
    [Range(0f, 1f)] public float shadowAlpha = 0.25f;
    public float shadowOffsetY = -5f;

    public Vector3 OriginalScale { get; private set; }
    private Tween glowTween;

    #region === SPRITE ===
    public void SetSprite(Sprite sprite)
    {
        food.sprite = sprite;
        OriginalScale = transform.Find("Container").localScale;
    }
    #endregion

    #region === SHADOW ===
    public void SetShadow(bool visible)
    {
        shadow.gameObject.SetActive(visible);

        if (!visible) return;

        shadow.color = new Color(0, 0, 0, shadowAlpha);
    }
    #endregion

    #region === LAYER VISUAL ===
    public void UpdateLayerVisual(int layer, int maxLayer)
    {
        UpdateOutline(layer, maxLayer);
        UpdateBrightness(layer);
    }

    private void UpdateOutline(int layer, int maxLayer)
    {
        glowTween?.Kill();

        if (layer <= 0)
        {
            outline.enabled = false;
            return;
        }

        outline.enabled = true;

        float t = Mathf.Clamp01(layer / (float)Mathf.Max(1, maxLayer));

        Color color = Color.Lerp(Color.white, Color.yellow, t);
        float alpha = Mathf.Lerp(0.25f, 0.85f, t);

        outline.effectColor =
            new Color(color.r, color.g, color.b, alpha);

        // Glow cho tile trên cùng
        if (layer == maxLayer && layer >= 3)
        {
            glowTween = outline
                .DOFade(alpha * 0.6f, 0.6f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void UpdateBrightness(int layer)
    {
        float brightness = Mathf.Clamp(
            0.7f + layer * 0.12f,
            0.7f,
            1f
        );

        food.color = new Color(brightness, brightness, brightness, 1f);
    }
    #endregion

    #region === INTERACTION ===
    public void PlayHover()
    {
        transform.DOScale(1.05f, 0.15f);
    }

    public void ResetScale()
    {
        transform.DOScale(1f, 0.15f);
    }
    #endregion
}
