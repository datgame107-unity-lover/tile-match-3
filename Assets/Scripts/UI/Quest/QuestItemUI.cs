// Scripts/UI/Quest/QuestItemUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // BẮT BUỘC: Thêm thư viện DOTween

public class QuestItemUI : MonoBehaviour
{
    [Header("Refs")]
    public Image tileIcon;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI progressText;
    public Image progressFill;
    public Button claimButton;
    public Image rewardIcon;
    public GameObject completedOverlay;

    private QuestDataSO _data;
    private ProgressService _progress;
    private bool _hasPlayedStampEffect = false; // Biến cờ để không chạy lại hiệu ứng khi vuốt/kéo ScrollView

    public void Init(QuestDataSO data, ProgressService progress)
    {
        _data = data;
        _progress = progress;

        questName.text = data.questName;

        if (data.targetTile != null && tileIcon != null)
            tileIcon.sprite = data.targetTile.sprite;

        // Xóa listener cũ tránh bị gọi đúp khi object được tái sử dụng (Object Pooling)
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaim);

        // Kiểm tra xem quest đã claim từ trước chưa
        var p = _progress.GetQuestProgress(_data.questID);
        _hasPlayedStampEffect = p.isClaimed;

        Refresh();
    }

    public void Refresh()
    {
        if (_data == null || _progress == null) return;

        var p = _progress.GetQuestProgress(_data.questID);
        int current = p.currentAmount;
        int target = _data.targetAmount;

        progressText.text = $"{current}/{target}";

        // Hiệu ứng thanh Progress chạy mượt
        if (progressFill != null)
        {
            progressFill.DOFillAmount((float)current / target, 0.4f).SetEase(Ease.OutCubic);
        }

        bool completed = current >= target;
        bool claimed = p.isClaimed;

        // Xử lý trạng thái hiển thị
        if (claimed)
        {
            claimButton.gameObject.SetActive(false);

            // Nếu nó được Refresh từ bên ngoài (ví dụ load data) mà chưa có effect thì bật bình thường
            if (completedOverlay != null && !completedOverlay.activeSelf)
            {
                completedOverlay.SetActive(true);
            }
        }
        else
        {
            claimButton.gameObject.SetActive(completed);
            claimButton.interactable = true; // Bật lại nút nếu nó đang bị tắt

            if (completedOverlay != null)
                completedOverlay.SetActive(false);
        }
    }

    private void OnClaim()
    {
        // 1. Khóa nút ngay lập tức để tránh người chơi bấm spam
        claimButton.interactable = false;

        // [TODO: Thêm SFX tiếng bấm nút ở đây]

        // 2. Hiệu ứng Nảy nút (Punch Scale)
        claimButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1)
            .OnComplete(() =>
            {
                // Gọi logic cộng tiền bên dưới Service
                _progress.ClaimQuest(_data.questID);

                // Tắt nút Claim đi
                claimButton.gameObject.SetActive(false);

                // 3. Gọi Hiệu ứng "Đóng Mộc" (Stamp)
                PlayStampEffect();
            });
    }

    private void PlayStampEffect()
    {
        if (completedOverlay == null || _hasPlayedStampEffect) return;

        _hasPlayedStampEffect = true;
        completedOverlay.SetActive(true);

        // Tự động thêm CanvasGroup nếu Object chưa có để làm hiệu ứng Fade (Mờ dần)
        CanvasGroup cg = completedOverlay.GetComponent<CanvasGroup>();
        if (cg == null) cg = completedOverlay.gameObject.AddComponent<CanvasGroup>();

        // Thiết lập trạng thái ban đầu: To gấp 2.5 lần và mờ tịt (alpha = 0)
        completedOverlay.transform.localScale = Vector3.one * 2.5f;
        cg.alpha = 0f;

        // Bắt đầu chuỗi hiệu ứng (Sequence)
        Sequence seq = DOTween.Sequence();

        // Hiện rõ dần...
        seq.Append(cg.DOFade(1f, 0.2f));

        // ...Và đập nhỏ xuống kích thước thật cực mạnh (OutBack tạo độ nảy đàn hồi)
        seq.Join(completedOverlay.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

        // Ngay khi vừa đập xuống, rung lắc toàn bộ thẻ Quest này một chút để tạo độ "lực"
        RectTransform myRect = GetComponent<RectTransform>();
        if (myRect != null)
        {
            // Rung lắc nhẹ 5 pixel trong 0.2 giây
            seq.Append(myRect.DOShakeAnchorPos(0.2f, strength: 5f, vibrato: 20));

            // [TODO: Chèn SFX tiếng nổ hoặc tiếng "Tada!" khớp với nhịp đập xuống này]
        }
    }
}