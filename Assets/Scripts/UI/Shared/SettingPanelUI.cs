// Scripts/UI/SettingsPanel.cs
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private ToggleSwitch soundToggle;
    [SerializeField] private ToggleSwitch vibrationToggle;
    [SerializeField] private ToggleSwitch musicToggle;

    [Header("Buttons")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button clearDataButton;
    [SerializeField] private Button backdropButton;

    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;

    private SoundService _sound;

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        // chỉ setup canvasGroup — chưa động đến ServiceLocator
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        // Start() đảm bảo GameBootstrapper.Awake() đã chạy xong
        _sound = ServiceLocator.Get<SoundService>();

        soundToggle?.SetValue(_sound.SfxOn, animate: false);
        vibrationToggle?.SetValue(_sound.VibrationOn, animate: false);
        musicToggle?.SetValue(_sound.MusicOn, animate: false);

        soundToggle.OnValueChanged += v => _sound.SetSFX(v);
        vibrationToggle.OnValueChanged += v => _sound.SetVibration(v);
        musicToggle.OnValueChanged += v => _sound.SetMusic(v);

        homeButton?.onClick.AddListener(OnHomeClicked);
        loginButton?.onClick.AddListener(OnLoginClicked);
        clearDataButton?.onClick.AddListener(OnClearDataClicked);
        backdropButton?.onClick.AddListener(Hide);
    }

    private void OnDestroy()
    {
        homeButton?.onClick.RemoveListener(OnHomeClicked);
        loginButton?.onClick.RemoveListener(OnLoginClicked);
        clearDataButton?.onClick.RemoveListener(OnClearDataClicked);
        backdropButton?.onClick.RemoveListener(Hide);
    }

    // ── Public API ────────────────────────────────────

    public void Show()
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.2f);
    }

    public void Hide()
    {
        if (canvasGroup == null) return;
        canvasGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    // ── Buttons ───────────────────────────────────────

    private void OnHomeClicked()
    {
        Hide();
        SceneLoader.LoadHome();
    }

    private void OnLoginClicked()
    {
        Debug.Log("[Settings] Login clicked");
    }

    private void OnClearDataClicked()
    {
        EventBus<ShowWarningEvent>.Publish(new ShowWarningEvent
        {
            config = new WarningConfig
            {
                message = "Clear all data? This cannot be undone.",
                confirmLabel = "Clear",
                cancelLabel = "Cancel",
                onConfirm = () =>
                {
                    // ServiceLocator.Get<ISaveService>().DeleteAll();
                    SceneLoader.LoadHome();
                },
            }
        });
    }
}