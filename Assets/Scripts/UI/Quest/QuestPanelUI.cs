// Scripts/UI/Quest/QuestPanelUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // BẮT BUỘC: Thêm thư viện DOTween

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
    public void Refresh()
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
        int target = 5; // dailyWinTarget (Có thể lấy từ ProgressService nếu bạn đã expose)
        float pct = _progress.GetDailyWinProgress();
        bool ready = _progress.IsDailyWinReady();
        bool claimed = _progress.IsDailyWinChestClaimed();

        if (dailyWinFill != null)
        {
            // Hiệu ứng thanh fill chạy mượt mà
            dailyWinFill.DOFillAmount(pct, 0.4f).SetEase(Ease.OutCubic);
        }

        if (dailyWinText != null)
            dailyWinText.text = $"{count}/{target}";

        if (dailyWinClaimButton != null)
        {
            // Nếu đủ điều kiện và chưa nhận -> Mở nút. Đã nhận hoặc chưa đủ -> Khóa nút.
            dailyWinClaimButton.interactable = ready && !claimed;
        }
    }

    private void RefreshQuestChest()
    {
        float pct = _progress.GetDailyQuestProgress();
        int total = _progress.GetTotalDailyQuest();
        int done = _progress.GetClaimedQuestCount();
        bool ready = _progress.IsDailyQuestReady();
        bool claimed = _progress.IsDailyQuestChestClaimed();

        if (questChestFill != null)
        {
            questChestFill.DOFillAmount(pct, 0.4f).SetEase(Ease.OutCubic);
        }

        if (questChestText != null)
            questChestText.text = $"{done}/{total}";

        if (questChestClaimButton != null)
        {
            questChestClaimButton.interactable = ready && !claimed;
        }
    }

    private void RefreshCurrency()
    {
        if (_currency == null) return;

        // Cập nhật Flower với hiệu ứng scale nảy lên
        if (flowerText != null)
        {
            string newFlowerText = _currency.Get(CurrencyType.Flower).ToString();
            if (flowerText.text != newFlowerText && !string.IsNullOrEmpty(flowerText.text))
            {
                flowerText.text = newFlowerText;
                flowerText.transform.DOKill(true); // Hủy anim cũ nếu đang chạy
                flowerText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 1f);
            }
            else
            {
                flowerText.text = newFlowerText; // Lần đầu bật UI
            }
        }

        // Cập nhật Diamond với hiệu ứng scale nảy lên
        if (diamondText != null)
        {
            string newDiamondText = _currency.Get(CurrencyType.Diamond).ToString();
            if (diamondText.text != newDiamondText && !string.IsNullOrEmpty(diamondText.text))
            {
                diamondText.text = newDiamondText;
                diamondText.transform.DOKill(true);
                diamondText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 1f);
            }
            else
            {
                diamondText.text = newDiamondText;
            }
        }
    }

    // ── Claim ────────────────────────────────────────────
    private void OnClaimDailyWin()
    {
        // Khóa nút ngay lập tức để tránh bấm đúp
        dailyWinClaimButton.interactable = false;

        // [TODO: Thêm AudioSource.PlayOneShot(buttonClickClip)]

        // Hiệu ứng nảy nút
        dailyWinClaimButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1)
            .OnComplete(() =>
            {
                // [TODO: Chạy Particle System rương mở tung hoặc VFX tiền bay ra ở đây]

                _progress.ClaimDailyWinChest(); // Gọi logic xử lý
                Refresh(); // Cập nhật lại UI (Tiền sẽ tự động nảy lên nhờ hàm RefreshCurrency)
            });
    }

    private void OnClaimQuestChest()
    {
        questChestClaimButton.interactable = false;

        questChestClaimButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1)
            .OnComplete(() =>
            {
                _progress.ClaimDailyQuestChest();
                Refresh();
            });
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

    // Khi tiền tệ thay đổi, chỉ cần cập nhật phần hiển thị tiền
    private void OnCurrencyChanged(CurrencyChangedEvent _) => RefreshCurrency();
}