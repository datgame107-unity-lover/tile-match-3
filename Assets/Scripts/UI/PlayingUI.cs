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
    public ShopUI shopUI;
    public Transform levelModePanel;public Transform infiniteModePanel;
    
    [Header("Texts")]
    public TextMeshProUGUI levelText;

    [Header("Buttons")]
    public Button settingButton;
    public GameObject shuffleButton;
    public GameObject undoButton;
    public GameObject powerUpButton;
    public GameObject hintButton;
   
    [Header("Settings UI")]
    public SettingUI settingUI;
    public Button settingUIOverlay; 

    
    private void Start()
    {
        levelText.text = PlayerPrefs.GetInt("level", 1).ToString();

        UpdateItemUI(shuffleButton, "Shuffle");
        UpdateItemUI(hintButton, "Hint");
        UpdateItemUI(undoButton, "Undo");
        UpdateItemUI(powerUpButton, "PowerUp");

        ShowGameplayUI();
        ActiveModeUI(GameManager.instance.gameMode);
    }

    private void OnEnable()
    {
        EventManager.OnCurrencyChanged += HandleCurrencyChanged;
        EventManager.OnStateChanged += HandleGameStateChanged;
        EventManager.OnPlayerWon += HandleNextLevel;
        EventManager.OnSettingButtonClicked += ShowSettingUI;
        EventManager.OnPlayerLost += PlayerLostHandler;
        EventManager.OnModeChanged += HandleModeChanged;
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        settingUIOverlay.onClick.AddListener(CloseSettingUI);

        SetupItemButton(hintButton, CurrencyType.Hint, tileManager.Hint);
        SetupItemButton(shuffleButton, CurrencyType.Shuffle, tileManager.Shuffle);
        SetupItemButton(undoButton, CurrencyType.Undo, tileManager.Undo);
        SetupItemButton(powerUpButton, CurrencyType.PowerUp, tileManager.PowerUp);
    }

    private void OnDisable()
    {
        EventManager.OnModeChanged -= HandleModeChanged;
        EventManager.OnCurrencyChanged -= HandleCurrencyChanged;
        settingButton.onClick.RemoveAllListeners();
        settingUIOverlay.onClick.RemoveAllListeners();
        hintButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        shuffleButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        undoButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        powerUpButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();

        EventManager.OnStateChanged -= HandleGameStateChanged;
        EventManager.OnPlayerWon -= HandleNextLevel;
        EventManager.OnSettingButtonClicked -= ShowSettingUI;
        EventManager.OnPlayerLost -= PlayerLostHandler;
    }

    #region --- Item Button Helper ---
    private void SetupItemButton(GameObject button, CurrencyType type, System.Action onUsed)
    {
        button.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            int count = CurrencyManager.Instance.Get(type);

            if (count <= 0)
            {
                ShowOutOfAbilityWarning();
                return;
            }


            button.transform.GetComponentInChildren<TextMeshProUGUI>().text = count == 0 ? "+" : count.ToString();
            onUsed?.Invoke();
        });
    }

    private void UpdateItemUI(GameObject button, string key)
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
                shopUI.gameObject.SetActive(!shopUI.gameObject.activeSelf);
            },
            refuseAction = () =>
            {
                warningPanel.gameObject.SetActive(false);
            }
        });
    }
    #endregion
    private void HandleModeChanged(GameMode mode)
    {
          
    }
    private void ActiveModeUI(GameMode mode)
    {
        if (mode == GameMode.Level)
        {
            levelModePanel.gameObject.SetActive(true);
            infiniteModePanel.gameObject.SetActive(false);
        }
        else
        {
            levelModePanel.gameObject.SetActive(false);
            infiniteModePanel.gameObject.SetActive(true);
        }
    }
    private void HandleCurrencyChanged(CurrencyType currencyType,int amount)
    {   
        switch(currencyType)
        {
            case CurrencyType.Hint:
            hintButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = amount == 0 ? "+" : amount.ToString();
                break;
            case CurrencyType.Shuffle:
                shuffleButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = amount == 0 ? "+" : amount.ToString();
                break;
            case CurrencyType.Undo:
                undoButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = amount == 0 ? "+" : amount.ToString();
                break;
            case CurrencyType.PowerUp:
                powerUpButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = amount == 0 ? "+" : amount.ToString();
                break;

        }    

    }

    #region --- Game State Handlers ---
    private void PlayerLostHandler()
    {
        playerLoseUI.gameObject.SetActive(true);
    }

    private void HandleNextLevel()
    {
        levelText.text = GameManager.instance.level.ToString();
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
        ChangeState(selectedTilePanel.gameObject, true);
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
