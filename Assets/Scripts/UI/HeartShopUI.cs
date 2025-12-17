using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartShopUI : MonoBehaviour
{

    public TextMeshProUGUI heartTimerText;
    public Button buyHeartButton;
    public Button watchAdsButton;
    public Button overlayButton;

    private void OnEnable()
    {
        buyHeartButton.onClick.AddListener(() =>
        {
            if (CurrencyManager.Instance.Get(CurrencyType.Heart)
    >= CurrencyManager.Instance.maxHeart)
            {
                return;
            }
            int price = int.Parse(
       buyHeartButton.GetComponentInChildren<TextMeshProUGUI>().text);
            bool success = CurrencyManager.Instance.Spend(CurrencyType.Flower, price);

            if (success)
            {
                CurrencyManager.Instance.Add(CurrencyType.Heart, 1);
            }
        });
        watchAdsButton.onClick.AddListener(() =>
        {

        });
        overlayButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }
    private void Update()
    {
        UpdateHeartTimer();
    }
    void UpdateHeartTimer()
    {
        if (CurrencyManager.Instance == null)
            return;

        int currentHeart = CurrencyManager.Instance.Get(CurrencyType.Heart);
        int maxHeart = CurrencyManager.Instance.maxHeart;

        heartTimerText.gameObject.SetActive(true);

        if (currentHeart >= maxHeart)
        {
            heartTimerText.text = "FULL";
            Destroy(gameObject);
            return;
        }

        float remain = CurrencyManager.Instance.GetHeartRemainTime();
        if (remain < 0) remain = 0;

        var t = System.TimeSpan.FromSeconds(remain);
        heartTimerText.text = $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

}
