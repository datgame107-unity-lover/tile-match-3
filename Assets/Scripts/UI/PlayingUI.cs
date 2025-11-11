using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayingUI : MonoBehaviour
{
    [Header("UI References")]
    public SelectedTilesUI selectedTilePanel;
    public PlayerWinUI playerWinUI;
    public TextMeshProUGUI levelText;
    public Button settingButton;
    public SettingUI settingUI;
    public Button settingUIOverlay; // overlay toàn màn hình

    private void Start()
    {
        // Hiển thị level hiện tại
        levelText.text = PlayerPrefs.GetInt("level", 1).ToString();

        // Nút mở Setting
        settingButton.onClick.AddListener(OnSettingButtonClicked);

        // Click ngoài Setting UI (overlay) để tắt
        settingUIOverlay.onClick.AddListener(() =>
        {
            CloseSettingUI();
        });

        // Khởi tạo trạng thái UI
        ShowGameplayUI();
    }

    private void OnEnable()
    {
        EventManager.OnStateChanged += HandleGameStateChanged;
        EventManager.OnPlayerWon += HandleNextLevel;
        EventManager.OnSettingButtonClicked += ShowSettingUI;
    }

    private void OnDisable()
    {
        EventManager.OnStateChanged -= HandleGameStateChanged;
        EventManager.OnPlayerWon -= HandleNextLevel;
        EventManager.OnSettingButtonClicked -= ShowSettingUI;
    }

    private void HandleNextLevel()
    {
        levelText.text = PlayerPrefs.GetInt("level", 1).ToString();
    }

    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Creating:
            case GameState.Playing:
                ShowGameplayUI();
                break;

            case GameState.Win:
                ShowWinUI();
                break;

            case GameState.Pause:
                ShowSettingUI();
                break;
        }
    }

    private void OnSettingButtonClicked()
    {
        EventManager.OnStateChanged(GameState.Pause);
    }

    private void ShowSettingUI()
    {
        ChangeState(settingUI.gameObject, true);
        ChangeState(settingUIOverlay.gameObject, true); // bật overlay
        ChangeState(selectedTilePanel.gameObject, false);
        ChangeState(playerWinUI.gameObject, false);
    }

    private void CloseSettingUI()
    {
        ChangeState(settingUI.gameObject, false);
        ChangeState(settingUIOverlay.gameObject, false); // tắt overlay
        EventManager.OnStateChanged(GameState.Playing); // quay lại gameplay
    }

    private void ShowWinUI()
    {
        ChangeState(selectedTilePanel.gameObject, false);
        ChangeState(playerWinUI.gameObject, true);
        ChangeState(settingUI.gameObject, false);
        ChangeState(settingUIOverlay.gameObject, false);
    }

    private void ShowGameplayUI()
    {
        ChangeState(selectedTilePanel.gameObject, true);
        ChangeState(playerWinUI.gameObject, false);
        ChangeState(settingUI.gameObject, false);
        ChangeState(settingUIOverlay.gameObject, false);
    }

    private void ChangeState(GameObject go, bool state)
    {
        if (go != null)
            go.SetActive(state);
    }
}
