// Scripts/UI/HighScoreUI.cs
using TMPro;
using UnityEngine;
using DG.Tweening;

public class HighScoreUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Định dạng số (ví dụ: D7 → 0012480)")]
    [SerializeField] private string scoreFormat = "D7";

    [Header("Animation khi score tăng")]
    [SerializeField] private float punchScale = 0.18f;
    [SerializeField] private float punchDuration = 0.3f;

    private int currentScore;
    private int highScore;


    private void Awake()
    {
        highScore = PlayerPrefs.GetInt(SaveKeys.EndLess.HighScore, 0);
    }

    private void OnEnable()
    {
        EventBus<ScoreAddedEvent>.Subscribe(OnScoreAdded);
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus<ScoreAddedEvent>.Unsubscribe(OnScoreAdded);
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnScoreAdded(ScoreAddedEvent e)
    {
        currentScore += e.amount;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(SaveKeys.EndLess.HighScore, highScore);
            PlayerPrefs.Save();
        }

        RefreshUI();
        PlayScorePunch();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.state == GameState.Lose || e.state == GameState.Win)
            return;

        if (e.state == GameState.Playing && currentScore == 0)
            RefreshUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = currentScore.ToString(scoreFormat);

        if (highScoreText != null)
            highScoreText.text = highScore.ToString(scoreFormat);
    }

    private void PlayScorePunch()
    {
        if (currentScoreText == null) return;
        currentScoreText.transform
            .DOPunchScale(Vector3.one * punchScale, punchDuration, 5, 0.5f)
            .SetEase(Ease.OutElastic);
    }
}