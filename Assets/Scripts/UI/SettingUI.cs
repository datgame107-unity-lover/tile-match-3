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
    public Slider soundSwitch;
    public Slider vibrationSwitch;
    public Slider musicSwitch;
    public Button homeButton;
    private void OnEnable()
    {
        // Gán listener cho từng switch
        soundSwitch.onValueChanged.AddListener((value) => HandleSwitchChanged(SwitchType.Sound, value));
        vibrationSwitch.onValueChanged.AddListener((value) => HandleSwitchChanged(SwitchType.Vibration, value));
        musicSwitch.onValueChanged.AddListener((value) => HandleSwitchChanged(SwitchType.Music, value));
        homeButton.onClick.AddListener(() => HomeButtonClick());
        // Cập nhật trạng thái khi mở Setting
        LoadSwitchState(soundSwitch, SwitchType.Sound);
        LoadSwitchState(vibrationSwitch, SwitchType.Vibration);
        LoadSwitchState(musicSwitch, SwitchType.Music);
    }

    private void OnDisable()
    {
        // Gỡ listener tránh memory leak
        soundSwitch.onValueChanged.RemoveAllListeners();
        vibrationSwitch.onValueChanged.RemoveAllListeners();
        musicSwitch.onValueChanged.RemoveAllListeners();
    }

    private void LoadSwitchState(Slider slider, SwitchType type)
    {
        bool isOn = PlayerPrefs.GetInt(type.ToString(), 1) == 1;
        slider.value = isOn ? 1 : 0;
    }
    public void HomeButtonClick()
    {
        SceneLoader.TargetScene = SceneEnum.Home; // đặt scene muốn load
        SceneManager.LoadScene(SceneEnum.Loading.ToString(), LoadSceneMode.Single);
    }
    private void HandleSwitchChanged(SwitchType type, float value)
    {
        bool isOn = value > 0.5f;
        PlayerPrefs.SetInt(type.ToString(), isOn ? 1 : 0);
        PlayerPrefs.Save();

        switch (type)
        {
            case SwitchType.Sound:
                HandleSound(isOn);
                break;

            case SwitchType.Vibration:
                HandleVibration(isOn);
                break;

            case SwitchType.Music:
                HandleMusic(isOn);
                break;
        }
    }

    private void HandleSound(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
        Debug.Log($"Sound: {(isOn ? "ON" : "OFF")}");
    }

    private void HandleVibration(bool isOn)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (isOn)
            Handheld.Vibrate(); // test rung khi bật
#endif
        Debug.Log($"Vibration: {(isOn ? "ON" : "OFF")}");
    }

    private void HandleMusic(bool isOn)
    {
        AudioSource bgm = GameObject.FindWithTag("BGM")?.GetComponent<AudioSource>();
        if (bgm)
        {
            if (isOn && !bgm.isPlaying)
                bgm.Play();
            else if (!isOn && bgm.isPlaying)
                bgm.Pause();
        }
        Debug.Log($"Music: {(isOn ? "ON" : "OFF")}");
    }
}
