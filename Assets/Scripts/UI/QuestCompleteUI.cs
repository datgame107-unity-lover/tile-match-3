using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompleteUI : MonoBehaviour
{
    [Header("UI")]
    public Image questIcon;
    public TextMeshProUGUI questName;
    public Image questProgress;
    public Image completeIcon;

    [Header("Tween Config")]
    public float slideDuration = 0.65f;
    public float stayDuration = 2f;
    public float hiddenOffsetY = 200f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector2 showPos;
    private Vector2 hiddenPos;
    private Tween currentTween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        showPos = rect.anchoredPosition;
        hiddenPos = showPos + Vector2.up * hiddenOffsetY;

        rect.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0;

        completeIcon.transform.localScale = Vector3.zero;
        questProgress.fillAmount = 0;
    }
    private void Start()
    {
        
    }
    public void Setup(QuestDataSO quest)
    {
        questIcon.sprite = quest.icon;
        questName.text = quest.name;
        Show(questIcon.sprite,questName.text);

    }
    public void Show(Sprite icon, string name)
    {
        questIcon.sprite = icon;
        questName.text = name;

        currentTween?.Kill();

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            rect.anchoredPosition = hiddenPos;
            canvasGroup.alpha = 0;
            completeIcon.transform.localScale = Vector3.zero;
            questProgress.fillAmount = 0;
        });

        // 1. Slide from top
        seq.Append(rect.DOAnchorPos(showPos, slideDuration)
            .SetEase(Ease.OutBack));

        seq.Join(canvasGroup.DOFade(1f, slideDuration));

        // 2. Progress fill
        seq.Append(questProgress.DOFillAmount(1f, 0.7f)
            .SetEase(Ease.OutCubic));
 
        // 3. Complete icon pop
        seq.Append(completeIcon.transform
            .DOScale(1f, 0.25f)
            .SetEase(Ease.OutBack));

        // 4. Stay
        seq.AppendInterval(stayDuration);

        // 5. Slide up & hide
        seq.Append(rect.DOAnchorPos(hiddenPos, 0.75f)
            .SetEase(Ease.InBack));


        currentTween = seq;
    }
}
