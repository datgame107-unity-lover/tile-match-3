using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SwitchType
{
    Sound,
    Vibration,
    Music
}

public class SettingUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider soundSwitch;
    public Slider vibrationSwitch;
    public Slider musicSwitch;
    public Button homeButton;
    public Button supportButton;
    public Button OverlayButton; // overlay full screen
    public RectTransform container; // vùng UI chính

    private void OnEnable()
    {
        // Gán listener
        soundSwitch.onValueChanged.AddListener((value) => HandleSwitchChanged(SwitchType.Sound, value));
        vibrationSwitch.onValueChanged.AddListener((value) => HandleSwitchChanged(SwitchType.Vibration, value));
        musicSwitch.onValueChanged.AddListener((value) => HandleSwitchChanged(SwitchType.Music, value));

        if (homeButton != null)
            homeButton.onClick.AddListener(HomeButtonClick);
        if (supportButton != null)
            supportButton.onClick.AddListener(SupportButtonClick);

        // Overlay click — chỉ tắt nếu click ngoài container
        OverlayButton.onClick.AddListener(() =>
        {
            Vector2 mousePos = Input.mousePosition;
            // Kiểm tra nếu click nằm ngoài container thì tắt
            if (!RectTransformUtility.RectangleContainsScreenPoint(container, mousePos, null))
            {
                this.gameObject.SetActive(false);
            }
        });

        // Load trạng thái mà không kích hoạt sự kiện
        LoadSwitchState();
    }

    private void OnDisable()
    {
        soundSwitch.onValueChanged.RemoveAllListeners();
        vibrationSwitch.onValueChanged.RemoveAllListeners();
        musicSwitch.onValueChanged.RemoveAllListeners();
        OverlayButton.onClick.RemoveAllListeners();
        homeButton?.onClick.RemoveAllListeners();
        supportButton?.onClick.RemoveAllListeners();
    }

    private void LoadSwitchState()
    {
        if (SoundManager.Instance == null) return;

        // Dùng SetValueWithoutNotify để không gọi HandleSwitchChanged
        soundSwitch.SetValueWithoutNotify(SoundManager.Instance.sfxOn ? 1 : 0);
        musicSwitch.SetValueWithoutNotify(SoundManager.Instance.musicOn ? 1 : 0);

        // Giả sử có lưu vibration ở PlayerPrefs
        vibrationSwitch.SetValueWithoutNotify(PlayerPrefs.GetInt("vibration", 1));
    }

    private void HandleSwitchChanged(SwitchType type, float value)
    {
        bool isOn = value > 0.5f;

        switch (type)
        {
            case SwitchType.Sound:
                SoundManager.Instance.SetSFX(isOn);
                break;

            case SwitchType.Vibration:
                PlayerPrefs.SetInt("vibration", isOn ? 1 : 0);
#if UNITY_ANDROID || UNITY_IOS
                if (isOn) Handheld.Vibrate();
#endif
                break;

            case SwitchType.Music:
                SoundManager.Instance.SetMusic(isOn);
                break;
        }
    }

    public void HomeButtonClick()
    {
        SceneLoader.TargetScene = SceneEnum.Home;
        SceneManager.LoadScene(SceneEnum.Loading.ToString(), LoadSceneMode.Single);
    }

    public void SupportButtonClick()
    {
        // TODO: xử lý nút Support
    }
}
