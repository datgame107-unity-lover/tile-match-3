// Scripts/UI/Home/HomePanelUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HomePanelUI : MonoBehaviour
{
    [Header("Button")]
    public Button settingsButton;
    public SettingsPanel settingsPanel;
    public Button shopButton;
    public ShopPanelUI shopPanel;

    [Header("Current Level")]
    public TextMeshProUGUI currentLevelText;
    public Button playButton;

    [Header("Game Mode Cards")]
    public Button levelModeCard;
    public Button endlessModeCard;
    public TextMeshProUGUI levelModeText;
    public TextMeshProUGUI endlessModeText;

    [Header("Global Rank")]
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI rankPercentText;

    [Header("Currency")]
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI flowerText;
    public TextMeshProUGUI diamondText;

    private ISaveService _save;
    private CurrencyService _currency;
    private ILeaderboardService _leaderboard;
    private GameMode _selectedMode = GameMode.Level;
    private LevelJsonService _level;

    private void OnEnable() => Refresh();
    private void OnDestroy() => UnsubscribeEvents();

    public void Init(
        ISaveService save,
        CurrencyService currency,
        ILeaderboardService leaderboard,
        LevelJsonService levelJsonService)
    {
        _save = save;
        _currency = currency;
        _leaderboard = leaderboard;
        _level = levelJsonService;
        playButton.onClick.AddListener(OnPlay);
        levelModeCard.onClick.AddListener(() =>
        {
            SelectMode(GameMode.Level);
        });
        endlessModeCard.onClick.AddListener(() =>
        {
            SelectMode(GameMode.EndLess);
        });
        settingsButton?.onClick.AddListener(() =>
        {
            settingsPanel.Show();
        });
        shopButton?.onClick.AddListener(() =>
        {
            shopPanel.Show();
        });


        SubscribeEvents();
        Refresh();
    }

    // ── Refresh ──────────────────────────────────────────
    private void Refresh()
    {
        if (_save == null) return;
        RefreshLevel();
        RefreshCurrency();
        RefreshRank();
        UpdateModeCards();
    }

    private void RefreshLevel()
    {
        int level = _save.GetInt(SaveKeys.Player.Level, 1);
        if (currentLevelText != null)
            currentLevelText.text = level.ToString();
    }
    private void RefreshCurrency()
    {
        if (_currency == null) return;
        if (heartText != null)
            heartText.text = _currency.Get(CurrencyType.Heart).ToString();
        if (flowerText != null)
            flowerText.text = _currency.Get(CurrencyType.Flower).ToString();
        if (diamondText != null)
            diamondText.text = _currency.Get(CurrencyType.Diamond).ToString();
    }

    private void RefreshRank()
    {
        if (_leaderboard == null) return;
        _leaderboard.GetPlayerRank(rank =>
        {
            if (rankText != null)
                rankText.text = rank > 0 ? $"#{rank}" : "—";
        });
        _leaderboard.GetPlayerRankPercent(pct =>
        {
            if (rankPercentText != null)
                rankPercentText.text = pct > 0 ? $"Top {pct}% this week" : "";
        });
        print("here");
    }

    // ── Mode select ──────────────────────────────────────
    private void SelectMode(GameMode mode)
    {
        _selectedMode = mode;
        UpdateModeCards();
    }

    private void UpdateModeCards()
    {
        // highlight card đang chọn
        var levelImg = levelModeCard.GetComponent<Image>();
        var endlessImg = endlessModeCard.GetComponent<Image>();

        if (levelImg != null)
            levelImg.color = _selectedMode == GameMode.Level
                ? new Color(1f, 0.75f, 0.1f)      // vàng active
                : new Color(0.95f, 0.4f, 0.65f);   // pink inactive

        if (endlessImg != null)
            endlessImg.color = _selectedMode == GameMode.EndLess
                ? new Color(1f, 0.75f, 0.1f)
                : new Color(0.95f, 0.4f, 0.65f);

        levelModeText.text = $"{_save.GetInt(SaveKeys.Player.Level)}/{_level.GetAllLevelIndices().Count}";
        endlessModeText.text = $"{_save.GetInt(SaveKeys.EndLess.HighScore)}";
    }

    // ── Play ─────────────────────────────────────────────
    private void OnPlay()
    {
        SceneLoader.LoadGame(_selectedMode);
    }

    // ── Events ───────────────────────────────────────────
    private void SubscribeEvents()
    {
        EventBus<CurrencyChangedEvent>.Subscribe(OnCurrencyChanged);
        EventBus<PlayerWonEvent>.Subscribe(OnPlayerWon);
    }

    private void UnsubscribeEvents()
    {
        EventBus<CurrencyChangedEvent>.Unsubscribe(OnCurrencyChanged);
        EventBus<PlayerWonEvent>.Unsubscribe(OnPlayerWon);
    }

    private void OnCurrencyChanged(CurrencyChangedEvent _) => RefreshCurrency();
    private void OnPlayerWon(PlayerWonEvent _) => RefreshLevel();
}