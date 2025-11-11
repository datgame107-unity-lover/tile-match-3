using DG.Tweening;
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
