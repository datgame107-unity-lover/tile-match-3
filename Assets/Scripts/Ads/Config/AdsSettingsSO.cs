using UnityEngine;

[CreateAssetMenu(
    fileName = "AdsSettings",
    menuName = "Ads/Ads Settings"
)]
public class AdsSettingsSO : ScriptableObject
{
    [Header("Global")]
    public bool enableAds = true;
    public bool forceDisableAds = false;

    [Header("Start Condition")]
    [Tooltip("Ads chỉ bắt đầu từ level này trở đi")]
    public int startLevel = 3;

    [Header("Cooldown")]
    [Tooltip("Số level phải cách nhau giữa 2 interstitial")]
    public int cooldownLevel = 2;
    [Tooltip("Cooldown cho Reward Button (giây)")]
    public float rewardedCooldownSeconds = 30f;

    [Header("Unity Ads")]
    public string androidGameId;
    public string iosGameId;

    [Header("Placements")]
    public string interstitialPlacement;
    public string rewardedPlacement;
    public string bannerPlacement;
    [Header("Banner")]

    public bool enableBanner; 

    [Header("Placements")]
    public string rewardedPlacementAndroid = "Rewarded_Android";
    public string rewardedPlacementIOS = "Rewarded_iOS";

    public string interstitialPlacementAndroid = "Interstitial_Android";
    public string interstitialPlacementIOS = "Interstitial_iOS";

    public string bannerPlacementAndroid = "Banner_Android";
    public string bannerPlacementIOS = "Banner_iOS";

    public bool testMode = true;
    public string GetRewardedPlacement()
    {
#if UNITY_ANDROID
    return rewardedPlacementAndroid;
#elif UNITY_IOS
    return rewardedPlacementIOS;
#else
        return rewardedPlacementAndroid;
#endif
    }

    public string GetInterstitialPlacement()
    {
#if UNITY_ANDROID
    return interstitialPlacementAndroid;
#elif UNITY_IOS
    return interstitialPlacementIOS;
#else
        return interstitialPlacementAndroid;
#endif
    }

    public string GetBannerPlacement()
    {
#if UNITY_ANDROID
    return bannerPlacementAndroid;
#elif UNITY_IOS
    return bannerPlacementIOS;
#else
        return bannerPlacementAndroid;
#endif
    }

    // ===== Helpers =====

    public string GetGameId()
    {
#if UNITY_ANDROID
        return androidGameId;
#elif UNITY_IOS
        return iosGameId;
#else
        return androidGameId;
#endif
    }
}
