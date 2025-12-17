using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerWinUI : MonoBehaviour
{
    public Button continueButton;
    public Button homeButton;
    private TileManager tileManager;
    public Transform flowerContainer;
    public Transform diamondContainer;
    public TextMeshProUGUI flowerText;
    public TextMeshProUGUI diamondText;
    public Image winImage;
    public Image dailyWinBar;
    public Image chestImage;
    public Sprite[] chestSprites;
    private void OnEnable()
    {
        if (QuestManager.Instance.IsDailyWinChestClaimed())
        {
            chestImage.sprite = QuestManager.Instance.IsDailyWinChestClaimed()?chestSprites[0]:chestSprites[1];
        }
        DOAnimationManager.ImageDropDown(winImage.GetComponent<RectTransform>());
        winImage.fillAmount = QuestManager.Instance.GetDailyWinProgress();

        EventManager.OnPlayerWon += HandlePlayerWin;
        continueButton.onClick.AddListener(() => {
            ContinueLevel();
        
        });
        homeButton.onClick.AddListener(() => {
            ReturnHome();

        });


        tileManager = GameObject.FindFirstObjectByType<TileManager>();
    }
    private void OnDisable()
    {
        continueButton.onClick?.RemoveAllListeners();
        homeButton.onClick?.RemoveAllListeners();
        EventManager.OnPlayerWon -= HandlePlayerWin;
    }

    public void ContinueLevel()
    {
        HideUI();
        tileManager.ContinueLevel();
    }
    public void ReturnHome()
    {
        HideUI();

        SceneLoader.TargetScene = SceneEnum.Home; // đặt scene muốn load
        SceneManager.LoadScene(SceneEnum.Loading.ToString(),LoadSceneMode.Single);
    }
    private void HandlePlayerWin()
    {
        gameObject.SetActive(true);
        
        flowerText.text = CurrencyManager.Instance.GetWinRewards(CurrencyType.Flower).ToString();
        diamondText.text = CurrencyManager.Instance.GetWinRewards(CurrencyType.Diamond).ToString();

        // 1. Flower shake
        DOAnimationManager.Shake(flowerContainer);

        // 2. Diamond shake
        DOVirtual.DelayedCall(0.25f, () =>
        {
            DOAnimationManager.Shake(diamondContainer);

            // 3. Flower text bounce
            DOVirtual.DelayedCall(0.5f, () =>
            {
                DOAnimationManager.TextBounce(flowerText.transform, 1.4f);

                // 4. Diamond text bounce
                DOVirtual.DelayedCall(0.15f, () =>
                {
                    DOAnimationManager.TextBounce(diamondText.transform, 1.4f);


                    float currentFill = dailyWinBar.fillAmount;

                    // giả sử mỗi win +20% (bạn tùy chỉnh)
                    float targetFill = Mathf.Clamp01(QuestManager.Instance.GetDailyWinProgress());

                    DOAnimationManager.MoveFillBar(dailyWinBar, targetFill, 0.6f);

                    // 6. Sau khi progress chạy xong → chest shake
                    DOVirtual.DelayedCall(0.6f, () =>
                    {
                        // Nếu bar chưa đầy → chest shake 1 lần
                        if (targetFill < 1f)
                        {
                            DOAnimationManager.ShakeOnce(chestImage.transform);
                        }
                        else
                        {
                            // Nếu bar đầy → chest lắc vô hạn
                            DOAnimationManager.ScaleBounce(chestImage.transform,Vector3.one,1.3f,0.5f);
                        }
                    });

                    // ============================================
                });
            });
        });
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }
}
