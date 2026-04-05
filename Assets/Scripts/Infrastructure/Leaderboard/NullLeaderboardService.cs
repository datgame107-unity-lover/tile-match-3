// Scripts/Infrastructure/Leaderboard/NullLeaderboardService.cs
using System;
using System.Collections.Generic;

public class NullLeaderboardService : ILeaderboardService
{
    public bool IsAuthenticated => throw new NotImplementedException();

    public void Authenticate(Action<bool> onResult)
    {
        throw new NotImplementedException();
    }

    public void GetPlayerRank(Action<int> onResult) => onResult?.Invoke(0);
    public void GetPlayerRankPercent(Action<int> onResult) => onResult?.Invoke(0);

    public void LoadTopScores(int count, Action<List<LeaderboardEntry>> onResult)
    {
        throw new NotImplementedException();
    }

    public void ShowLeaderboard()
    {
        throw new NotImplementedException();
    }

    public void SubmitScore(long score) { }

    public void SubmitScore(long score, Action<bool> onResult = null)
    {
        throw new NotImplementedException();
    }
}