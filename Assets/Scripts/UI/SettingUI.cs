using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SwitchType
{
    Sound,
    Vibration,
    Music
}

public class SettingUI : MonoBehaviour
{
    [Header("UI References")]
    public Button soundSwitch;
    public Button vibrationSwitch;
    public Button musicSwitch;

    public Button homeButton;
    public Button supportButton;
    public Button OverlayButton;

    public RectTransform container;
    public Sprite[] switchImages; // [0] = ON, [1] = OFF

    private void OnEnable()
    {
        soundSwitch.onClick.AddListener(() => HandleSwitchChanged(SwitchType.Sound));
        vibrationSwitch.onClick.AddListener(() => HandleSwitchChanged(SwitchType.Vibration));
        musicSwitch.onClick.AddListener(() => HandleSwitchChanged(SwitchType.Music));

        homeButton?.onClick.AddListener(HomeButtonClick);
        supportButton?.onClick.AddListener(SupportButtonClick);

        OverlayButton.onClick.AddListener(() => gameObject.SetActive(false));

        SetImage();
    }

    private void OnDisable()
    {
        soundSwitch.onClick.RemoveAllListeners();
        vibrationSwitch.onClick.RemoveAllListeners();
        musicSwitch.onClick.RemoveAllListeners();
        OverlayButton.onClick.RemoveAllListeners();
        homeButton?.onClick.RemoveAllListeners();
        supportButton?.onClick.RemoveAllListeners();
    }

    private void SetImage()
    {
        soundSwitch.image.sprite = SoundManager.Instance.sfxOn ? switchImages[0] : switchImages[1];
        musicSwitch.image.sprite = SoundManager.Instance.musicOn ? switchImages[0] : switchImages[1];
        vibrationSwitch.image.sprite = SoundManager.Instance.vibrationOn ? switchImages[0] : switchImages[1];
    }

    private void HandleSwitchChanged(SwitchType type)
    {
        switch (type)
        {
            case SwitchType.Sound:
                SoundManager.Instance.SetSFX(!SoundManager.Instance.sfxOn);
                soundSwitch.image.sprite = SoundManager.Instance.sfxOn ? switchImages[0] : switchImages[1];
                break;

            case SwitchType.Vibration:
                bool vib = PlayerPrefs.GetInt("vibration", 1) == 1;
                bool newVibState = !vib;
                PlayerPrefs.SetInt("vibration", newVibState ? 1 : 0);

#if UNITY_ANDROID || UNITY_IOS
                if (newVibState) Handheld.Vibrate();
#endif

                vibrationSwitch.image.sprite = newVibState ? switchImages[0] : switchImages[1];
                SoundManager.Instance.vibrationOn = newVibState;
                break;

            case SwitchType.Music:
                SoundManager.Instance.SetMusic(!SoundManager.Instance.musicOn);
                musicSwitch.image.sprite = SoundManager.Instance.musicOn ? switchImages[0] : switchImages[1];
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
        // TODO
    }
}
