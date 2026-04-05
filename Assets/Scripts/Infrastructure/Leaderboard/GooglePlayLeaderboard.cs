//// Scripts/Infrastructure/Leaderboard/GooglePlayLeaderboard.cs
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//#if UNITY_ANDROID
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames.BasicApi.Leaderboards;
//#endif

//public class GooglePlayLeaderboard : ILeaderboardService
//{
//    private const string LeaderboardId = "YOUR_LEADERBOARD_ID"; // thay bằng ID thật từ Google Play Console

//    public bool IsAuthenticated
//    {
//        get
//        {
//#if UNITY_ANDROID
//            return PlayGamesPlatform.Instance.IsAuthenticated();
//#else
//            return false;
//#endif
//        }
//    }

//    public GooglePlayLeaderboard()
//    {
//#if UNITY_ANDROID
//        PlayGamesPlatform.Activate();
//#endif
//        SubscribeEvents();
//    }

//    // ── Auth ──────────────────────────────────────────
//    public void Authenticate(Action<bool> onResult)
//    {
//#if UNITY_ANDROID
//        PlayGamesPlatform.Instance.Authenticate(status =>
//        {
//            bool success = status == SignInStatus.Success;
//            Debug.Log(success
//                ? "[GooglePlay] Authenticated"
//                : $"[GooglePlay] Auth failed: {status}");
//            onResult?.Invoke(success);
//        });
//#else
//        onResult?.Invoke(false);
//#endif
//    }

//    // ── Submit ────────────────────────────────────────
//    public void SubmitScore(long score, Action<bool> onResult = null)
//    {
//#if UNITY_ANDROID
//        if (!IsAuthenticated)
//        {
//            Debug.LogWarning("[GooglePlay] Not authenticated");
//            onResult?.Invoke(false);
//            return;
//        }

//        PlayGamesPlatform.Instance.ReportScore(
//            score,
//            LeaderboardId,
//            success =>
//            {
//                Debug.Log(success
//                    ? $"[GooglePlay] Score submitted: {score}"
//                    : "[GooglePlay] Score submit failed");

//                if (success)
//                    EventBus<LeaderboardSubmittedEvent>.Publish(
//                        new LeaderboardSubmittedEvent { score = (int)score });

//                onResult?.Invoke(success);
//            });
//#else
//        onResult?.Invoke(false);
//#endif
//    }

//    // ── Show UI ───────────────────────────────────────
//    public void ShowLeaderboard()
//    {
//#if UNITY_ANDROID
//        if (!IsAuthenticated)
//        {
//            Authenticate(_ => ShowLeaderboard());
//            return;
//        }
//        PlayGamesPlatform.Instance.ShowLeaderboardUI(LeaderboardId);
//#endif
//    }

//    // ── Load scores ───────────────────────────────────
//    public void LoadTopScores(int count, Action<List<LeaderboardEntry>> onResult)
//    {
//#if UNITY_ANDROID
//        PlayGamesPlatform.Instance.LoadScores(
//            LeaderboardId,
//            LeaderboardStart.TopScores,
//            count,
//            LeaderboardCollection.Public,
//            LeaderboardTimeSpan.AllTime,
//            data =>
//            {
//                if (data.Valid)
//                {
//                    var entries = new List<LeaderboardEntry>();
//                    foreach (var score in data.Scores)
//                    {
//                        entries.Add(new LeaderboardEntry
//                        {
//                            playerName = score.userID,
//                            score = score.value,
//                            rank = (int)score.rank,
//                        });
//                    }
//                    EventBus<LeaderboardLoadedEvent>.Publish(
//                        new LeaderboardLoadedEvent());
//                    onResult?.Invoke(entries);
//                }
//                else
//                {
//                    Debug.LogError("[GooglePlay] LoadScores failed");
//                    onResult?.Invoke(null);
//                }
//            });
//#else
//        onResult?.Invoke(null);
//#endif
//    }

//    // ── Private ───────────────────────────────────────
//    private void SubscribeEvents()
//    {
//        EventBus<HighScoreChangedEvent>.Subscribe(OnHighScoreChanged);
//    }

//    private void OnHighScoreChanged(HighScoreChangedEvent e)
//    {
//        if (!IsAuthenticated) return;
//        SubmitScore(e.score);
//    }

//    public void Dispose()
//    {
//        EventBus<HighScoreChangedEvent>.Unsubscribe(OnHighScoreChanged);
//    }
//}