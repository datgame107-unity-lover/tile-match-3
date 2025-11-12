using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private int menuCount = 3;
    [SerializeField] private float tweenDuration = 0.35f;
    [SerializeField] private Button playButton;
    [SerializeField] private Button chaneModeButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingUI;
    private int currentIndex = 1    ;

    private void Start()
    {
        SnapToPage(currentIndex, true); // Đặt page bắt đầu
        scrollRect.vertical = false;    // Khóa trục dọc

        playButton.onClick.AddListener(() =>
        {
            SceneLoader.TargetScene = SceneEnum.GameScene;
            SceneManager.LoadScene(SceneEnum.Loading.ToString(),LoadSceneMode.Single);
        });
        chaneModeButton.onClick.AddListener(() =>
        {
            ChangeMode();
        });

        settingButton.onClick.AddListener(() =>
        {
            settingUI.SetActive(!settingUI.activeSelf);
        });
    }
    private void ChangeMode()
    {
        GameMode currentMode = GameManager.instance.gameMode;
        // Đổi chế độ
        currentMode = currentMode == GameMode.Level ? GameMode.Infinite : GameMode.Level;

        GameManager.instance.ChangeMode(currentMode);
        // Lấy component text
        TextMeshProUGUI buttonText = chaneModeButton.GetComponentInChildren<TextMeshProUGUI>();

        // Gán text hiển thị
        buttonText.text = currentMode.ToString();

        // Đổi màu button và màu text tương ứng
        if (currentMode == GameMode.Level)
        {
            // Chế độ Level
            ColorUtility.TryParseHtmlString("#51DA4D", out Color bgColor);
            ColorUtility.TryParseHtmlString("#F5F5F5", out Color textColor);
            chaneModeButton.image.color = bgColor;
            buttonText.color = textColor;
        }
        else
        {
            // Chế độ Infinite (màu khác khi đổi)
            ColorUtility.TryParseHtmlString("#3B82F6", out Color bgColor); // xanh dương
            ColorUtility.TryParseHtmlString("#FFFFFF", out Color textColor); // trắng
            chaneModeButton.image.color = bgColor;
            buttonText.color = textColor;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float normalizedPos = scrollRect.horizontalNormalizedPosition;

        // Tính index gần nhất
        int targetIndex = Mathf.RoundToInt((1 - normalizedPos) * (menuCount - 1));

        // Giới hạn index
        targetIndex = Mathf.Clamp(targetIndex, 0, menuCount - 1);

        // Snap khi thả
        SnapToPage(targetIndex);
    }

    private void SnapToPage(int index, bool immediate = false)
    {
        currentIndex = index;

        float targetPos = (float)index / (menuCount - 1);
        targetPos = 1 - targetPos; // do normalized Pos của ScrollRect đi từ 1 -> 0

        if (immediate)
        {
            scrollRect.horizontalNormalizedPosition = targetPos;
        }
        else
        {
            DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                        x => scrollRect.horizontalNormalizedPosition = x,
                        targetPos, tweenDuration)
                   .SetEase(Ease.OutCubic);
        }
    }
    public void GoToPage(int pageIndex)
    {   

        pageIndex = Mathf.Clamp(pageIndex, 0, menuCount - 1);
        SnapToPage(pageIndex);
    }
}
