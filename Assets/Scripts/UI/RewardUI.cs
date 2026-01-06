using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardUI : MonoBehaviour
{
    public GameObject questReward;   // prefab reward
    public Transform container;      // container chứa reward
    public Transform giftBox;        // image hộp quà ở giữa
    public Button closeButton;       // nút close

    private void Awake()
    {
        // Ẩn nút close lúc đầu
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(CloseUI);
        }
    }

    public void LoadRewardItems(List<QuestReward> reward)
    {
        PopGiftBox();

        // Ẩn nút close khi load
        if (closeButton != null)
            closeButton.gameObject.SetActive(false);

        // Xóa reward cũ
        foreach (Transform child in container)
            Destroy(child.gameObject);

        float totalDelay = 0f; // lưu tổng delay để bật nút sau cùng

        for (int i = 0; i < reward.Count; i++)
        {
            QuestReward rewardItem = reward[i];
            Transform questRewardUI = Instantiate(questReward, container).transform;

            questRewardUI.Find("Base/RewardIcon").GetComponent<Image>().sprite = rewardItem.icon;
            questRewardUI.Find("RewardAmount").GetComponent<TextMeshProUGUI>().text = rewardItem.amount.ToString();

            RectTransform rect = questRewardUI.GetComponent<RectTransform>();
            Vector3 finalPos = rect.anchoredPosition;
            rect.anchoredPosition = finalPos + new Vector3(0, 200f, 0);

            CanvasGroup canvasGroup = questRewardUI.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = questRewardUI.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            float delay = 0.1f * i;
            totalDelay = delay;

            rect.DOAnchorPosY(finalPos.y, 1f).SetEase(Ease.OutBounce).SetDelay(delay)
                .OnComplete(() =>
                {
                    rect.DOScale(Vector3.one * 1.2f, 0.2f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() => rect.localScale = Vector3.one);
                });

            canvasGroup.DOFade(1f, 0.6f).SetDelay(delay);
        }

        // Hiển thị nút close với animation
        float showCloseDelay = totalDelay + 1f; // sau khi reward rơi xong
        DOVirtual.DelayedCall(showCloseDelay, () =>
        {
            if (closeButton != null)
            {
                closeButton.gameObject.SetActive(true);

                // Animation scale nảy
                closeButton.transform.localScale = Vector3.zero;
                closeButton.transform.DOScale(Vector3.one, 0.4f)
                             .SetEase(Ease.OutBack);

                // Fade in (nếu có CanvasGroup)
                CanvasGroup cg = closeButton.GetComponent<CanvasGroup>();
                if (cg == null) cg = closeButton.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.DOFade(1f, 0.4f);
            }
        });
    }

    private void PopGiftBox()
    {
        giftBox.localScale = Vector3.zero;
        giftBox.DOScale(Vector3.one * 1.3f, 0.3f)
               .SetEase(Ease.OutBack)
               .OnComplete(() => giftBox.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutSine));
    }

    private void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
