public class AdsController
{
    private AdsSettingsSO settings;
    private AdsCooldown cooldown = new AdsCooldown();

    public AdsController(AdsSettingsSO settings)
    {
        this.settings = settings;

        AdsManager.Instance.OnRewardedClosed += OnRewardedShown;
    }


    // ===== INTERSTITIAL =====

    public bool ShouldShowInterstitial(
        AdsPlacement placement,
        int currentLevel)
    {
        if (!settings.enableAds) return false;
        if (settings.forceDisableAds) return false;
        if (currentLevel < settings.startLevel) return false;

        switch (placement)
        {
            case AdsPlacement.LevelComplete:
            case AdsPlacement.LevelFail:
            case AdsPlacement.Restart:
            case AdsPlacement.BackToMenu:
                return cooldown.CanShowInterstitial(
                    currentLevel,
                    settings.cooldownLevel
                );
        }

        return false;
    }

    public void OnInterstitialShown(int level)
    {
        cooldown.MarkInterstitialShown(level);
    }

    // ===== REWARDED BUTTON =====

    public bool CanClickRewardedButton()
    {
        if (!settings.enableAds) return false;
        if (settings.forceDisableAds) return false;

        return cooldown.CanShowRewarded(
            settings.rewardedCooldownSeconds
        );
    }

    private void OnRewardedShown()
    {
        cooldown.MarkRewardedShown();
    }

}
