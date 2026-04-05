// Scripts/UI/LoseUIPanel.cs
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseUIPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Penalty")]
    [SerializeField] private RectTransform heartIcon;
    [SerializeField] private TextMeshProUGUI heartPenaltyText;

    [Header("Buttons")]
    [SerializeField] private Button playOnButton;
    [SerializeField] private Button giveUpButton;

    [Header("Play On Cost")]
    [SerializeField] private TextMeshProUGUI playOnCostText;
    [SerializeField] private GameObject adIcon;
    [SerializeField] private GameObject diamondIcon;
    [SerializeField] private int diamondCost = 30;

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float punchDuration = 0.4f;

    private CurrencyService _currency;
    private bool _adsAvailable;

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        _currency = ServiceLocator.Get<CurrencyService>();

        EventBus<PlayerLostEvent>.Subscribe(OnLose);
        playOnButton?.onClick.AddListener(OnPlayOnClicked);
        giveUpButton?.onClick.AddListener(OnGiveUpClicked);

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        EventBus<PlayerLostEvent>.Unsubscribe(OnLose);
        playOnButton?.onClick.RemoveListener(OnPlayOnClicked);
        giveUpButton?.onClick.RemoveListener(OnGiveUpClicked);
    }

    // ── Handler ──────────────────────────────────────

    private void OnLose(PlayerLostEvent evt)
    {
        if (heartPenaltyText) heartPenaltyText.text = "-1";

        //_adsAvailable = AdsService.IsRewardedAdReady();
        RefreshPlayOnButton();
        PlayShowAnim();
    }

    // ── Play On button UI ─────────────────────────────

    private void RefreshPlayOnButton()
    {
        if (adIcon) adIcon.SetActive(_adsAvailable);
        if (diamondIcon) diamondIcon.SetActive(!_adsAvailable);

        if (playOnCostText)
            playOnCostText.text = _adsAvailable ? "" : diamondCost.ToString();

        if (playOnButton)
            playOnButton.interactable = _adsAvailable ||
                _currency.Get(CurrencyType.Diamond) >= diamondCost;
    }

    // ── Anim ─────────────────────────────────────────

    private void PlayShowAnim()
    {
        var seq = DOTween.Sequence();

        if (canvasGroup)
        {
            seq.AppendCallback(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
            seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        }

        if (heartIcon)
            seq.Append(heartIcon.DOPunchScale(Vector3.one * 0.3f, punchDuration, 5, 0.5f));

        seq.Play();
    }

    // ── Hide ─────────────────────────────────────────

    private void Hide()
    {
        DOTween.Kill(canvasGroup);
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    // ── Buttons ───────────────────────────────────────

    private void OnPlayOnClicked()
    {
        if (_adsAvailable)
        {
            //AdsService.ShowRewardedAd(onSuccess: () =>
            //{
            //    Hide();
            //    EventBus<PlayOnEvent>.Publish(new PlayOnEvent()); // ← Publish, không phải Subscribe
            //});
        }
        else
        {
            if (_currency.Get(CurrencyType.Diamond) < diamondCost) return;
            _currency.Spend(CurrencyType.Diamond, diamondCost);
            Hide();
            EventBus<PlayOnEvent>.Publish(new PlayOnEvent()); // ← Publish, không phải Subscribe
        }
    }

    private void OnGiveUpClicked()
    {
        EventBus<ShowWarningEvent>.Publish(new ShowWarningEvent
        {
            config = new WarningConfig
            {
                message = "Are you sure you want to give up?",
                confirmLabel = "Reset",
                cancelLabel = "Home",
                onConfirm = () => SceneLoader.LoadGame(SceneLoader.PendingMode),
                onCancel = () => SceneLoader.LoadHome(),
            }
        });
    }
}