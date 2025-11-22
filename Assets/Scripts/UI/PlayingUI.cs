using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayingUI : MonoBehaviour
{
    public TileManager tileManager;

    [Header("UI Panels")]
    public SelectedTilesUI selectedTilePanel;
    public PlayerWinUI playerWinUI;
    public PlayerLoseUI playerLoseUI;
    public WarningPanel warningPanel;

    [Header("Texts")]
    public TextMeshProUGUI levelText;

    [Header("Buttons")]
    public Button settingButton;
    public Button shuffleButton;
    public Button undoButton;
    public Button powerUpButton;
    public Button hintButton;

    [Header("Settings UI")]
    public SettingUI settingUI;
    public Button settingUIOverlay; // overlay toàn màn hình

    private void Start()
    {
        // Set level text
        levelText.text = PlayerPrefs.GetInt("level", 1).ToString();

        // Set số lượng item UI
        UpdateItemUI(shuffleButton, "Shuffle");
        UpdateItemUI(hintButton, "Hint");
        UpdateItemUI(undoButton, "Undo");
        UpdateItemUI(powerUpButton, "PowerUp");

        ShowGameplayUI();
    }

    private void OnEnable()
    {
        EventManager.OnStateChanged += HandleGameStateChanged;
        EventManager.OnPlayerWon += HandleNextLevel;
        EventManager.OnSettingButtonClicked += ShowSettingUI;
        EventManager.OnPlayerLost += PlayerLostHandler;

        settingButton.onClick.AddListener(OnSettingButtonClicked);
        settingUIOverlay.onClick.AddListener(CloseSettingUI);

        // --- Setup item buttons ---
        SetupItemButton(hintButton, "Hint", tileManager.OnHintButton);
        SetupItemButton(shuffleButton, "Shuffle", tileManager.OnShuffleButton);
        SetupItemButton(undoButton, "Undo", tileManager.OnUndoButton);
        SetupItemButton(powerUpButton, "PowerUp", tileManager.OnPowerUpButton);
    }

    private void OnDisable()
    {
        settingButton.onClick.RemoveAllListeners();
        settingUIOverlay.onClick.RemoveAllListeners();
        hintButton.onClick.RemoveAllListeners();
        shuffleButton.onClick.RemoveAllListeners();
        undoButton.onClick.RemoveAllListeners();
        powerUpButton.onClick.RemoveAllListeners();

        EventManager.OnStateChanged -= HandleGameStateChanged;
        EventManager.OnPlayerWon -= HandleNextLevel;
        EventManager.OnSettingButtonClicked -= ShowSettingUI;
        EventManager.OnPlayerLost -= PlayerLostHandler;
    }

    #region --- Item Button Helper ---
    private void SetupItemButton(Button button, string key, System.Action onUse)
    {
        button.onClick.AddListener(() =>
        {
            int count = PlayerPrefs.GetInt(key, 0);

            if (count <= 0)
            {
                ShowOutOfAbilityWarning();
                return;
            }

            count--;
            PlayerPrefs.SetInt(key, count);
            PlayerPrefs.Save();

            button.transform.GetComponentInChildren<TextMeshProUGUI>().text = count == 0 ? "+" : count.ToString();

            onUse?.Invoke();
        });
    }

    private void UpdateItemUI(Button button, string key)
    {
        int count = PlayerPrefs.GetInt(key, 0);
        button.transform.GetComponentInChildren<TextMeshProUGUI>().text = count == 0 ? "+" : count.ToString();
    }

    private void ShowOutOfAbilityWarning()
    {
        warningPanel.ShowWarning(new WarningData()
        {
            warningType = WarningType.Delete,
            message = "Out Of Ability!",
            agreeText = "Shop",
            refuseText = "Return",
            agreeAction = () =>
            {
                // Vào shop logic nếu cần
                Debug.Log("Open Shop");
            },
            refuseAction = () =>
            {
                this.gameObject.SetActive(false);
            }
        });
    }
    #endregion

    #region --- Game State Handlers ---
    private void PlayerLostHandler()
    {
        playerLoseUI.gameObject.SetActive(true);
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
    #endregion

    #region --- UI Panels ---
    private void ShowSettingUI()
    {
        ChangeState(settingUI.gameObject, true);
        ChangeState(settingUIOverlay.gameObject, true);
        ChangeState(selectedTilePanel.gameObject, false);
        ChangeState(playerWinUI.gameObject, false);
    }

    private void CloseSettingUI()
    {
        ChangeState(settingUI.gameObject, false);
        ChangeState(settingUIOverlay.gameObject, false);
        EventManager.OnStateChanged(GameState.Playing);
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
    #endregion
}
