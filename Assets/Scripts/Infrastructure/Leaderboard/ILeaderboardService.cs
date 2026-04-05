// Scripts/Infrastructure/Leaderboard/ILeaderboardService.cs
using System;
using System.Collections.Generic;

public interface ILeaderboardService
{
    bool IsAuthenticated { get; }

    void Authenticate(Action<bool> onResult);
    void SubmitScore(long score, Action<bool> onResult = null);
    void ShowLeaderboard();
    void LoadTopScores(int count, Action<List<LeaderboardEntry>> onResult); 
    void GetPlayerRank(Action<int> onResult);
    void GetPlayerRankPercent(Action<int> onResult);
}

public class LeaderboardEntry
{
    public string playerName;
    public long score;
    public int rank;
}