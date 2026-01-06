using System;
using UnityEngine;
using UnityEngine.Advertisements;
using static UnityEngine.Advertisements.Advertisement;

public class UnityAdsProvider :
    IAdsProvider,
    IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    private AdsSettingsSO settings;

#if UNITY_ANDROID
    private const string INTERSTITIAL = "Interstitial_Android";
    private const string REWARDED = "Rewarded_Android";
    private const string BANNER = "Banner_Android";
#elif UNITY_IOS
    private const string INTERSTITIAL = "Interstitial_iOS";
    private const string REWARDED = "Rewarded_iOS";
    private const string BANNER = "Banner_iOS";
#else
    // === EDITOR / OTHER PLATFORM SAFE ===
    private const string INTERSTITIAL = "Interstitial_Android";
    private const string REWARDED = "Rewarded_Android";
    private const string BANNER = "Banner_Android";
#endif

    private Action rewardedCallback;
    private Action failCallback;

    public UnityAdsProvider(AdsSettingsSO settings)
    {
        this.settings = settings;
    }

    // ================= INIT =================

    public void Initialize()
    {
        string gameId = settings.GetGameId();

        if (string.IsNullOrEmpty(gameId))
        {
            Debug.LogError("UnityAds GameId is EMPTY");
            return;
        }

        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(
                gameId,
                settings.testMode,
                this
            );
        }
    }



    public void OnInitializationComplete()
    {
        LoadAds();
    }

    public void OnInitializationFailed(
        UnityAdsInitializationError error,
        string message
    )
    {
        Debug.LogError($"UnityAds Init Failed: {error} - {message}");
    }

    // ================= LOAD =================

    private void LoadAds()
    {
        Advertisement.Load(INTERSTITIAL, this);
        Advertisement.Load(REWARDED, this);

        if (settings.enableBanner)
        {
            BannerLoadOptions options = new BannerLoadOptions();
            Advertisement.Banner.Load(BANNER, options);
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"UnityAds Loaded: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(
        string placementId,
        UnityAdsLoadError error,
        string message
    )
    {
        Debug.LogWarning($"UnityAds Load Failed: {placementId}");
    }

    // ================= SHOW =================

    public bool IsReady(AdsType type)
    {
        return type switch
        {
            AdsType.Interstitial => Advertisement.isInitialized,
            AdsType.Rewarded => Advertisement.isInitialized,
            AdsType.Banner => Advertisement.isInitialized,
            _ => false
        };
    }

    public void Show(
        AdsType type,
        Action onSuccess = null,
        Action onFail = null
    )
    {
        rewardedCallback = onSuccess;
        failCallback = onFail;

        switch (type)
        {
            case AdsType.Interstitial:
                Advertisement.Show(INTERSTITIAL, this);
                break;

            case AdsType.Rewarded:
                Advertisement.Show(REWARDED, this);
                break;

            case AdsType.Banner:
                Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
                Advertisement.Banner.Show(BANNER);
                break;
        }
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    // ================= CALLBACK =================

    public void OnUnityAdsShowComplete(
        string placementId,
        UnityAdsShowCompletionState state
    )
    {
        if (placementId == REWARDED)
        {
            if (state == UnityAdsShowCompletionState.COMPLETED)
                rewardedCallback?.Invoke();
            else
                failCallback?.Invoke();
        }

        Advertisement.Load(placementId, this);
    }

    public void OnUnityAdsShowFailure(
        string placementId,
        UnityAdsShowError error,
        string message
    )
    {
        failCallback?.Invoke();
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
}
