using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DangerBarUI : MonoBehaviour
{
    public Image fill;
    public float lerpSpeed = 6f;

    private Coroutine lerpRoutine;

    public void SetValue(float value)
    {
        // Khởi động Lerp mới
        if (lerpRoutine != null)
            StopCoroutine(lerpRoutine);

        lerpRoutine = StartCoroutine(LerpFill(value));
    }

    private IEnumerator LerpFill(float target)
    {
        float startFill = fill.fillAmount;
        Color startColor = fill.color;
        Color targetColor;

        if (target > 0.8f)
            targetColor = Color.red;
        else if (target > 0.5f)
            targetColor = new Color(1f, 0.6f, 0f); // cam
        else
            targetColor = Color.green;

        while (Mathf.Abs(fill.fillAmount - target) > 0.001f)
        {
            fill.fillAmount = Mathf.Lerp(fill.fillAmount, target, Time.deltaTime * lerpSpeed);
            fill.color = Color.Lerp(fill.color, targetColor, Time.deltaTime * lerpSpeed);

            yield return null;
        }

        fill.fillAmount = target;
        fill.color = targetColor;

        lerpRoutine = null;
    }
}
