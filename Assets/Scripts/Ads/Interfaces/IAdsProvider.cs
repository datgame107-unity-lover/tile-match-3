using System;

public interface IAdsProvider
{
    void Initialize();

    bool IsReady(AdsType type);

    void Show(
        AdsType type,
        Action onSuccess = null,
        Action onFail = null
    );

    void HideBanner();
}
