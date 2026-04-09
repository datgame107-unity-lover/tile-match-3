using UnityEngine;

// Tự động thêm CanvasGroup nếu bạn quên gắn trong Inspector
[RequireComponent(typeof(CanvasGroup))]
public class EndlessModePanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        EventBus<EndlessModeStartedEvent>.Subscribe(OnEndlessStarted);
    }

    private void OnDisable()
    {
        EventBus<EndlessModeStartedEvent>.Unsubscribe(OnEndlessStarted);
    }

    private void Start()
    {
        HidePanel(); // Tàng hình khi mới bắt đầu
    }

    private void OnEndlessStarted(EndlessModeStartedEvent e)
    {
        ShowPanel(); // Hiện lên khi nhận được sự kiện
    }

    private void HidePanel()
    {
        _canvasGroup.alpha = 0f;               // Tàng hình
        _canvasGroup.interactable = false;     // Không cho bấm nút
        _canvasGroup.blocksRaycasts = false;   // Cho phép click xuyên qua nó xuống lớp UI bên dưới
    }

    private void ShowPanel()
    {
        _canvasGroup.alpha = 1f;               // Hiện rõ
        _canvasGroup.interactable = true;      // Có thể tương tác
        _canvasGroup.blocksRaycasts = true;    // Chặn click
    }
}