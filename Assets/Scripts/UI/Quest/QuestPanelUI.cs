// Scripts/UI/Quest/QuestPanelUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestPanelUI : MonoBehaviour
{
    [Header("Daily Wins")]
    public Image dailyWinFill;
    public TextMeshProUGUI dailyWinText;
    public Button dailyWinClaimButton;

    [Header("Quest Chest")]
    public Image questChestFill;
    public TextMeshProUGUI questChestText;
    public Button questChestClaimButton;

    [Header("Quest List")]
    public Transform questListContainer;
    public GameObject questItemPrefab;

    [Header("Currency")]
    public TextMeshProUGUI flowerText;
    public TextMeshProUGUI diamondText;

    private ProgressService _progress;
    private CurrencyService _currency;
    private List<QuestDataSO> _quests;
    private readonly List<QuestItemUI> _questItems = new List<QuestItemUI>();

    private void OnEnable() => Refresh();
    private void OnDestroy() => UnsubscribeEvents();

    public void Init(
        ProgressService progress,
        CurrencyService currency
        )
    {
        _progress = progress;
        _currency = currency;
        _quests = progress.GetDailyQuests();

        dailyWinClaimButton.onClick.AddListener(OnClaimDailyWin);
        questChestClaimButton.onClick.AddListener(OnClaimQuestChest);

        SubscribeEvents();
        BuildQuestList();
        Refresh();
    }

    // ── Build list ───────────────────────────────────────
    private void BuildQuestList()
    {   
        foreach (Transform t in questListContainer)
            Destroy(t.gameObject);
        _questItems.Clear();

        foreach (var quest in _quests)
        {
            var go = Instantiate(questItemPrefab, questListContainer);
            var item = go.GetComponent<QuestItemUI>();
            item.Init(quest, _progress);
            _questItems.Add(item);
        }
    }

    // ── Refresh ──────────────────────────────────────────
    private void Refresh()
    {
        if (_progress == null) return;
        RefreshDailyWin();
        RefreshQuestChest();
        RefreshCurrency();
        foreach (var item in _questItems) item.Refresh();
    }

    private void RefreshDailyWin()
    {
        int count = _progress.GetDailyWinCount();
        int target = 5; // dailyWinTarget
        float pct = _progress.GetDailyWinProgress();
        bool ready = _progress.IsDailyWinReady();
        bool claimed = _progress.IsDailyWinChestClaimed();

        if (dailyWinFill != null)
            dailyWinFill.fillAmount = pct;

        if (dailyWinText != null)
            dailyWinText.text = $"{count}/{target}";

        if (dailyWinClaimButton != null)
            dailyWinClaimButton.interactable = ready && !claimed;
    }

    private void RefreshQuestChest()
    {
        float pct = _progress.GetDailyQuestProgress();
        int total = _progress.GetTotalDailyQuest();
        int done = _progress.GetClaimedQuestCount();
        bool ready = _progress.IsDailyQuestReady();
        bool claimed = _progress.IsDailyQuestChestClaimed();

        if (questChestFill != null)
            questChestFill.fillAmount = pct;

        if (questChestText != null)
            questChestText.text = $"{done}/{total}";

        if (questChestClaimButton != null)
            questChestClaimButton.interactable = ready && !claimed;
    }

    private void RefreshCurrency()
    {
        if (_currency == null) return;
        if (flowerText != null)
            flowerText.text = _currency.Get(CurrencyType.Flower).ToString();
        if (diamondText != null)
            diamondText.text = _currency.Get(CurrencyType.Diamond).ToString();
    }

    // ── Claim ────────────────────────────────────────────
    private void OnClaimDailyWin()
    {
        _progress.ClaimDailyWinChest();
        Refresh();
    }

    private void OnClaimQuestChest()
    {
        _progress.ClaimDailyQuestChest();
        Refresh();
    }

    // ── Events ───────────────────────────────────────────
    private void SubscribeEvents()
    {
        EventBus<QuestCompletedEvent>.Subscribe(OnQuestCompleted);
        EventBus<QuestClaimedEvent>.Subscribe(OnQuestClaimed);
        EventBus<PlayerWonEvent>.Subscribe(OnPlayerWon);
        EventBus<CurrencyChangedEvent>.Subscribe(OnCurrencyChanged);
    }

    private void UnsubscribeEvents()
    {
        EventBus<QuestCompletedEvent>.Unsubscribe(OnQuestCompleted);
        EventBus<QuestClaimedEvent>.Unsubscribe(OnQuestClaimed);
        EventBus<PlayerWonEvent>.Unsubscribe(OnPlayerWon);
        EventBus<CurrencyChangedEvent>.Unsubscribe(OnCurrencyChanged);
    }

    private void OnQuestCompleted(QuestCompletedEvent _) => Refresh();
    private void OnQuestClaimed(QuestClaimedEvent _) => Refresh();
    private void OnPlayerWon(PlayerWonEvent _) => RefreshDailyWin();
    private void OnCurrencyChanged(CurrencyChangedEvent _) => RefreshCurrency();
}
