using UnityEngine;

public class AdsCooldown
{
    private int lastInterstitialLevel = -999;
    private float lastRewardedTime = -999f;

    // ===== INTERSTITIAL (LEVEL) =====

    public bool CanShowInterstitial(
        int currentLevel,
        int cooldownLevel)
    {
        return currentLevel - lastInterstitialLevel >= cooldownLevel;
    }

    public void MarkInterstitialShown(int level)
    {
        lastInterstitialLevel = level;
    }

    // ===== REWARDED (TIME) =====

    public bool CanShowRewarded(float cooldownSeconds)
    {
        return Time.time - lastRewardedTime >= cooldownSeconds;
    }

    public void MarkRewardedShown()
    {
        lastRewardedTime = Time.time;
    }

    // ===== UTILS =====

    public void Reset()
    {
        lastInterstitialLevel = -999;
        lastRewardedTime = -999f;
    }
}
