// Scripts/Application/ScoreService.cs

public class ScoreService
{
    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    private readonly ISaveService save;

    public ScoreService(ISaveService save)
    {
        this.save = save;
        HighScore = save.GetInt(SaveKeys.EndLess.HighScore, 0);

        EventBus<ScoreAddedEvent>.Subscribe(OnScoreAdded);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        EventBus<ScoreChangedEvent>.Publish(new ScoreChangedEvent { total = 0});
    }

    public void Dispose()
    {
        EventBus<ScoreAddedEvent>.Unsubscribe(OnScoreAdded);
    }

    // ── Private ───────────────────────────────────────
    private void OnScoreAdded(ScoreAddedEvent e)
    {
        CurrentScore += e.amount;
        EventBus<ScoreChangedEvent>.Publish(new ScoreChangedEvent {  total =CurrentScore});

        if (CurrentScore <= HighScore) return;

        HighScore = CurrentScore;
        save.SetInt(SaveKeys.EndLess.HighScore, HighScore);
        save.Save();
        EventBus<HighScoreChangedEvent>.Publish(new HighScoreChangedEvent {score = HighScore });
    }
}