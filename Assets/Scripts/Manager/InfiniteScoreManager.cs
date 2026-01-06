using UnityEngine;

public class InfiniteScoreManager : MonoBehaviour
{
    public static InfiniteScoreManager Instance;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    private const string HIGH_SCORE_KEY = "INFINITE_HIGH_SCORE";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        
    }

    private void OnEnable()
    {
        EventManager.OnScoreAdd += AddScore;
    }

    private void OnDisable()
    {
        EventManager.OnScoreAdd -= AddScore;
    }

    private void AddScore(int amount)
    {
        CurrentScore += amount;
        EventManager.OnScoreChanged?.Invoke(CurrentScore);

        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
            EventManager.OnHighScoreChanged?.Invoke(HighScore);
            
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        EventManager.OnScoreChanged?.Invoke(CurrentScore);
    }
}
