using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdsManager : MonoBehaviour,
    IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    public static AdsManager Instance;

    public AdsSettingsSO settings;

    private Action rewardedCallback;
    private bool isRewardedLoaded;
    private bool isLoadingRewarded;
    public event Action OnRewardedClosed;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Initialize();
    }

    // ===== INIT =====

    void Initialize()
    {
        if (settings == null)
        {
            Debug.LogError("AdsSettingsSO is NULL");
            return;
        }

        string gameId = settings.GetGameId();

        if (string.IsNullOrEmpty(gameId))
        {
            Debug.LogError("Unity Ads GameId is EMPTY");
            return;
        }

        Advertisement.Initialize(gameId, settings.testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Initialized");
        PreloadRewarded();
    }

    public void OnInitializationFailed(
        UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Init Failed: {error} - {message}");
    }

    // ===== REWARDED =====

    void PreloadRewarded()
    {
        if (isLoadingRewarded || isRewardedLoaded)
            return;

        isLoadingRewarded = true;
        Advertisement.Load(settings.rewardedPlacement, this);
    }

    public bool IsRewardedReady()
    {
        return isRewardedLoaded;
    }

    public void ShowRewarded(Action onSuccess)
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Unity Ads not initialized");
            return;
        }

        if (!isRewardedLoaded)
        {
            Debug.LogWarning("Rewarded not ready");
            PreloadRewarded();
            return;
        }

        rewardedCallback = onSuccess;
        Advertisement.Show(settings.rewardedPlacement, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == settings.rewardedPlacement)
        {
            isRewardedLoaded = true;
            isLoadingRewarded = false;
            Debug.Log("Rewarded loaded");
        }
    }

    public void OnUnityAdsFailedToLoad(
        string placementId,
        UnityAdsLoadError error, string message)
    {
        if (placementId == settings.rewardedPlacement)
        {
            isRewardedLoaded = false;
            isLoadingRewarded = false;
            Debug.LogError($"Rewarded Load Failed: {error}");
        }
    }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowFailure(
        string placementId,
        UnityAdsShowError error, string message)
    {
        if (placementId == settings.rewardedPlacement)
        {
            rewardedCallback = null;
            isRewardedLoaded = false;
            PreloadRewarded();
            Debug.LogError($"Rewarded Show Failed: {error}");
        }
    }

    public void OnUnityAdsShowComplete(
      string placementId,
      UnityAdsShowCompletionState state)
    {
        if (placementId != settings.rewardedPlacement)
            return;

        isRewardedLoaded = false;

        // BÁO: rewarded ad đã đóng (kể cả skip)
        OnRewardedClosed?.Invoke();

        if (state == UnityAdsShowCompletionState.COMPLETED)
        {
            rewardedCallback?.Invoke();
        }

        rewardedCallback = null;
    }


}
