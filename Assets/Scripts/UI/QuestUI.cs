using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public QuestManager manager;
    public GameObject questItemPrefab;
    public Transform contentParent;
    public Image dailyQuestProgress;
    public Button giftButton;

    public Sprite[] spriteLists;

    [SerializeField] private Transform giftTransform; // Hộp quà UI
    [SerializeField] private GameObject rewardUI;      // Panel hiển thị reward

    private void Start()
    {
        if (manager == null)
        {
            manager = QuestManager.Instance;
        }
    }

    private void OnEnable()
    {
        dailyQuestProgress.fillAmount = QuestManager.Instance.GetDailyQuestProgress();
        if (QuestManager.Instance.IsDailyQuestChestClaimed())
        {
            giftButton.GetComponent<Image>().sprite = spriteLists[1];
            giftButton.interactable = false;

        }
        else
        {
            giftButton.GetComponent<Image>().sprite = spriteLists[0];
            giftButton.interactable = true;


        }

        LoadQuests();
        EventManager.OnQuestClaimed += HandleQuestClaimed;

        giftButton.onClick.AddListener(OnGiftButtonClicked);


    }
    private void OnDisable()
    {
        EventManager.OnQuestClaimed -= HandleQuestClaimed;
    }

    private void HandleQuestClaimed(QuestDataSO quest)
    {
        float currentFill = dailyQuestProgress.fillAmount;

        float targetFill = Mathf.Clamp01(QuestManager.Instance.GetDailyQuestProgress());

        DOAnimationManager.MoveFillBar(dailyQuestProgress, targetFill, 0.6f);
    }
    public void LoadQuests()
    {
        // Xóa item cũ
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Tạo mới UI item
        foreach (var questData in manager.dailyQuestsData)
        {
            ProgressData progress = manager.GetProgress(questData.questID);

            QuestItemUI questItem = Instantiate(questItemPrefab, contentParent)
                                        .GetComponent<QuestItemUI>();

            questItem.Setup(questData);
        }
    }

    private void OnGiftButtonClicked()
    {
        // Chỉ mở khi progress max
        if (dailyQuestProgress.fillAmount >= 1f)
        {
            giftButton.interactable = false;
            giftButton.GetComponent<Image>().sprite = spriteLists[1];
            OpenGift();
        }
    }


    private void OpenGift()
    {
        ShakeGiftRotation();
    }

    private void ShakeGiftRotation()
    {
        Vector3 originalRotation = giftTransform.localEulerAngles;
        float angle = 15f; // góc lắc trái/phải
        int loops = 2;     // số lần lắc qua lại

        // Lắc xoay qua trái → phải → lặp lại
        giftTransform.DOLocalRotate(new Vector3(0, 0, angle), 0.2f)
                     .SetLoops(loops * 2, LoopType.Yoyo) // mỗi lần qua + lại = 2 loops
                     .SetEase(Ease.InOutSine)
                     .OnComplete(() =>
                     {
                         giftTransform.localEulerAngles = originalRotation; // reset góc
                         ShowReward();
                     });
    }

    private void ShowReward()
    {
        // Hiển thị reward với scale animation
        rewardUI.SetActive(true);

        RewardUI rewardUIComponent = rewardUI.GetComponent<RewardUI>();
        rewardUIComponent.LoadRewardItems(QuestManager.Instance.GetDailyQuestRewards());

        rewardUI.transform.localScale = Vector3.zero;
        rewardUI.transform.DOScale(Vector3.one, 0.5f)
                          .SetEase(Ease.OutBack);

        // Reset progress
        QuestManager.Instance.ClaimDailyQuestChest();

        // Cập nhật lại UI quests
        LoadQuests();
    }

}
