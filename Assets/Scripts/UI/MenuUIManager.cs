using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public int menuCount = 3;
    public float tweenDuration = 0.35f;
    public Button playButton;
    public Button changeModeButton;
    public Button settingButton;
    public Button shopButton;
    public GameObject settingUI;
    public GameObject shopUI;
    public Button levelCreatorButton;
    private int currentIndex = 1    ;
    private void Start()
    {
        SnapToPage(currentIndex, true); 
        scrollRect.vertical = false;   

        playButton.onClick.AddListener(() =>
        {
            SceneLoader.TargetScene = SceneEnum.GameScene;
            SceneManager.LoadScene(SceneEnum.Loading.ToString(),LoadSceneMode.Single);
        });
        changeModeButton.onClick.AddListener(() =>
        {
            ChangeMode();
        });

        settingButton.onClick.AddListener(() =>
        {
            settingUI.SetActive(!settingUI.activeSelf);
        });

        shopButton.onClick.AddListener(() =>
        {
            shopUI.SetActive(true);
        });
        levelCreatorButton.onClick.AddListener(() =>
        {
            SceneLoader.TargetScene = SceneEnum.LevelCreator; // đặt scene muốn load
            SceneManager.LoadScene(SceneEnum.Loading.ToString(), LoadSceneMode.Single);
        });
    }
    private void ChangeMode()
    {
        GameMode currentMode = GameManager.instance.gameMode;
        currentMode = currentMode == GameMode.Level ? GameMode.Infinite : GameMode.Level;

        GameManager.instance.ChangeMode(currentMode);
        TextMeshProUGUI buttonText = changeModeButton.GetComponentInChildren<TextMeshProUGUI>();

        buttonText.text = currentMode.ToString();

        if (currentMode == GameMode.Level)
        {
            ColorUtility.TryParseHtmlString("#51DA4D", out Color bgColor);
            ColorUtility.TryParseHtmlString("#F5F5F5", out Color textColor);
            changeModeButton.image.color = bgColor;
            buttonText.color = textColor;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#3B82F6", out Color bgColor); // xanh dương
            ColorUtility.TryParseHtmlString("#FFFFFF", out Color textColor); // trắng
            changeModeButton.image.color = bgColor;
            buttonText.color = textColor;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float normalizedPos = scrollRect.horizontalNormalizedPosition;

        int targetIndex = Mathf.RoundToInt((1 - normalizedPos) * (menuCount - 1));

        targetIndex = Mathf.Clamp(targetIndex, 0, menuCount - 1);

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
