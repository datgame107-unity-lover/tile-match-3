using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{   
    public static CurrencyUI Instance;

    public TextMeshProUGUI flowerText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI diamondText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // giữ qua các scene
    }
    private void Start()
    {
        flowerText.text = CurrencyManager.Instance.Get(CurrencyType.Flower).ToString();
        heartText.text = CurrencyManager.Instance.Get(CurrencyType.Heart).ToString();
        diamondText.text = CurrencyManager.Instance.Get(CurrencyType.Diamond).ToString();
    }

    private void OnEnable()
    {
        EventManager.OnCurrencyChanged += CurrencyChangedHandler;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyChanged -= CurrencyChangedHandler;
    }

    private void CurrencyChangedHandler(CurrencyType type, int value)
    {
        switch (type)
        {
            case CurrencyType.Flower:
                flowerText.text = value.ToString();
                break;

            case CurrencyType.Heart:
                heartText.text = value.ToString();
                break;

            case CurrencyType.Diamond:
                diamondText.text = value.ToString();
                break;
        }
    }
}
