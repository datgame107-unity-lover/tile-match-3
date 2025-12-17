using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemUI : MonoBehaviour
{
    [Header("UI References")]
    // Bạn vẫn dùng .Find() hoặc kéo thả đều được. Ở đây mình dùng [SerializeField] cho an toàn.
    public Image iconImage;
    public TextMeshProUGUI descriptionText;

    [Header("Progress")]
    public Image progressBarFill;
    public TextMeshProUGUI progressText;

    [Header("Button")]
     public Button claimButton;
    public TextMeshProUGUI buttonText; // Text bên trong nút (Vd: Giá tiền hoặc chữ "Claim")
    public Button rewardButton;

    public GameObject questItemDetailsUI;
    private QuestDataSO questData;
    public void Setup(QuestDataSO data)
    {
        questData = data;

        if (data.icon != null)
            iconImage.sprite = data.icon;

        descriptionText.text = data.description;

        Refresh();
    }
    private void Refresh()
    {
        ProgressData progress =
            QuestManager.Instance.GetProgress(questData.questID);

        float ratio = (float)progress.currentAmount / questData.targetAmount;
        progressBarFill.fillAmount = Mathf.Clamp01(ratio);
        progressText.text = $"{progress.currentAmount}/{questData.targetAmount}";

        claimButton.onClick.RemoveAllListeners();
        rewardButton.onClick.RemoveAllListeners();

        if (progress.isClaimed)
        {
            claimButton.interactable = false;
        }
        else if (progress.currentAmount >= questData.targetAmount)
        {
            claimButton.interactable = true;
            claimButton.onClick.AddListener(() =>
            {
                RectTransform from = rewardButton.GetComponent<RectTransform>();
                Canvas canvas = CurrencyUI.Instance.GetComponentInParent<Canvas>();

                foreach (var reward in questData.rewards)
                {
                    if (!IsFlyCurrency(reward.type))
                        continue;

                    RectTransform target =
                        CurrencyUI.Instance.GetTarget(reward.type);

                    if (target == null)
                        continue;

                    RewardFlyUtil.Fly(
                        reward.icon,
                        from,
                        target,
                        canvas,
                        0.6f
                    );
                }

                QuestManager.Instance.ClaimReward(questData.questID);
                Refresh();
            });

           

        }
        else
        {
            claimButton.interactable = false;
        }

        rewardButton.onClick.AddListener(() =>
        {
            rewardButton.transform
                .DOPunchScale(Vector3.one * 0.1f, 0.2f);

            GameObject ui = Instantiate(
                questItemDetailsUI,
                rewardButton.transform.position,Quaternion.identity,transform);

            ui.transform.localScale = Vector3.one * 0.85f;
            ui.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

            ui.GetComponent<QuestItemDetailsUI>()
              .LoadReward(questData.rewards.ToList());
        });
    }

    void PlayClaimAnimation()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMoveY(transform.localPosition.y + 15f, 0.25f));
        seq.Join(cg.DOFade(0f, 0.25f));
        seq.OnComplete(() =>
        {   
            
            Refresh();
            cg.alpha = 1f;
            transform.DOLocalMoveY(transform.localPosition.y - 15f, 0f);
        });
    }
bool IsFlyCurrency(CurrencyType type)
{
    return type == CurrencyType.Flower
        || type == CurrencyType.Heart
        || type == CurrencyType.Diamond;
}

}
