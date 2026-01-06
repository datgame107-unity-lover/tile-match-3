using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InfiniteModeUI : MonoBehaviour
{
    [Header("UI")]
    public Image comboBar;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Tween Config")]
    public float punchScale = 0.25f;
    public float punchDuration = 0.25f;

    private Tween comboTween;
    private Tween scoreTween;
    private Tween highScoreTween;

    private int lastCombo = -1;


    private void Start()
    {
        if (InfiniteScoreManager.Instance != null)
            highScoreText.text = InfiniteScoreManager.Instance.HighScore.ToString("N0");

        comboBar.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.OnComboChanged += UpdateCombo;
        EventManager.OnComboReset += ResetCombo;
        EventManager.OnScoreChanged += UpdateScore;
        EventManager.OnHighScoreChanged += UpdateHighScore;
    }

    private void OnDisable()
    {
        EventManager.OnComboChanged -= UpdateCombo;
        EventManager.OnComboReset -= ResetCombo;
        EventManager.OnScoreChanged -= UpdateScore;
        EventManager.OnHighScoreChanged -= UpdateHighScore;
    }



    private void UpdateCombo(int combo, float fill)
    {
        if (combo <= 0) return;

        comboText.text = $"x{combo}";
        comboBar.fillAmount = fill;
        comboBar.gameObject.SetActive(true);

        if (combo == lastCombo) return; 

        lastCombo = combo;

        comboTween?.Kill();
        comboText.transform.localScale = Vector3.one;

        comboTween = comboText.transform
            .DOScale(1.5f, 0.4f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }




    private void ResetCombo()
    {
        comboTween?.Kill();
        comboText.text = string.Empty;
        comboBar.fillAmount = 0f;
        comboBar.gameObject.SetActive(false);
    }


    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString("N0");

        scoreTween?.Kill();
        scoreTween = scoreText.transform
            .DOPunchScale(Vector3.one * 0.15f, 0.2f, 5, 0.8f);
    }


    private void UpdateHighScore(int highScore)
    {
        highScoreText.text = highScore.ToString("N0");

        highScoreTween?.Kill();
        highScoreTween = highScoreText.transform
            .DOPunchScale(Vector3.one * 0.3f, 0.35f, 8, 1f);
    }
}
