using System;
using UnityEngine;

public class AdMobProvider : IAdsProvider
{
    private AdsSettingsSO settings;

    public AdMobProvider(AdsSettingsSO settings)
    {
        this.settings = settings;
    }

    public void Initialize()
    {
        Debug.Log("AdMob Init");
        // MobileAds.Initialize(...)
    }

    public bool IsReady(AdsType type)
    {
        // check loaded ads
        return true;
    }

    public void Show(
        AdsType type,
        Action onSuccess = null,
        Action onFail = null
    )
    {
        Debug.Log("Show AdMob: " + type);

        switch (type)
        {
            case AdsType.Banner:
                break;

            case AdsType.Interstitial:
                onSuccess?.Invoke();
                break;

            case AdsType.Rewarded:
                onSuccess?.Invoke();
                break;
        }
    }

    public void HideBanner()
    {
        Debug.Log("Hide Banner");
    }
}
