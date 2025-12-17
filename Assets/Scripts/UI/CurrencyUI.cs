using DG.Tweening;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    public static CurrencyUI Instance;

    [Header("Texts")]
    public TextMeshProUGUI flowerText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI diamondText;

    [Header("Targets (RectTransform)")]
    public RectTransform flowerTarget;
    public RectTransform heartTarget;
    public RectTransform diamondTarget;

    [Header("Heart Timer")]
    public TextMeshProUGUI heartTimerText;

    int flowerValue;
    int heartValue;
    int diamondValue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshAll();
    }

    private void OnEnable()
    {
        EventManager.OnCurrencyChanged += CurrencyChangedHandler;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyChanged -= CurrencyChangedHandler;
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
            return;
        }

        float remain = CurrencyManager.Instance.GetHeartRemainTime();
        if (remain < 0) remain = 0;

        var t = System.TimeSpan.FromSeconds(remain);
        heartTimerText.text = $"{t.Minutes:D2}:{t.Seconds:D2}";
    }


    void RefreshAll()
    {
        flowerText.text = CurrencyManager.Instance.Get(CurrencyType.Flower).ToString();
        heartText.text = CurrencyManager.Instance.Get(CurrencyType.Heart).ToString();
        diamondText.text = CurrencyManager.Instance.Get(CurrencyType.Diamond).ToString();
    }

    private void CurrencyChangedHandler(CurrencyType type, int newValue)
    {
        if (!IsUICurrency(type))
            return;

        var text = GetText(type);
        if (text == null)
            return;

        int from = GetCachedValue(type);

        DOTween.Kill(text); // tránh tween chồng

        DOTween.To(
            () => from,
            x =>
            {
                from = x;
                text.text = x.ToString();
                SetCachedValue(type, x);
            },
            newValue,
            0.4f
        ).SetEase(Ease.OutCubic);

        // feedback nhẹ
        text.transform.DOPunchScale(Vector3.one * 0.12f, 0.15f);
    }
    int GetCachedValue(CurrencyType type)
    {
        return type switch
        {
            CurrencyType.Flower => flowerValue,
            CurrencyType.Heart => heartValue,
            CurrencyType.Diamond => diamondValue,
            _ => 0
        };
    }

    void SetCachedValue(CurrencyType type, int v)
    {
        switch (type)
        {
            case CurrencyType.Flower: flowerValue = v; break;
            case CurrencyType.Heart: heartValue = v; break;
            case CurrencyType.Diamond: diamondValue = v; break;
        }
    }
    public bool IsUICurrency(CurrencyType type)
    {
        return type == CurrencyType.Flower
            || type == CurrencyType.Heart
            || type == CurrencyType.Diamond;
    }

    public RectTransform GetTarget(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Flower: return flowerTarget;
            case CurrencyType.Heart: return heartTarget;
            case CurrencyType.Diamond: return diamondTarget;
        }
        return null;
    }

    TextMeshProUGUI GetText(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Flower: return flowerText;
            case CurrencyType.Heart: return heartText;
            case CurrencyType.Diamond: return diamondText;
        }
        return null;
    }
}
