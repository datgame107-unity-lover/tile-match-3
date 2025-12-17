using DG.Tweening;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class ShopCurrencyUI : MonoBehaviour
{   
    
    public Image currencyIcon;
    public Text currencyAmount;
    public Text currencyPrice;
    public Button purchaseButton;
    public ShopManager _shopManager;
    private ShopItemSO currencyItem;
    private void Start()
    {
        _shopManager = FindFirstObjectByType<ShopManager>();
    }
    private void OnEnable()
    {
        purchaseButton.onClick.AddListener(() =>
        {
            if (!CurrencyUI.Instance.IsUICurrency(currencyItem.currencyType))
            {
                OnPurchase();
                return;
            }

            RectTransform from = purchaseButton.GetComponent<RectTransform>();
            RectTransform target =
                CurrencyUI.Instance.GetTarget(currencyItem.currencyType);

            Canvas canvas =
                CurrencyUI.Instance.GetComponentInParent<Canvas>().rootCanvas;

            RewardFlyUtil.Fly(
                currencyItem.icon,
                from,
                target,
                canvas,
                0.6f
            );

            DOVirtual.DelayedCall(0.15f, OnPurchase);
        });
    }
    private void OnDisable()
    {
        purchaseButton.onClick?.RemoveAllListeners();
    }

    public void SetupCurrency(ShopItemSO currency)
    {   
        currencyItem = currency;
        currencyIcon.sprite = currency.icon;
        currencyAmount.text = currency.quantity.ToString();
        currencyPrice.text = currency.buyWith.price.ToString();
    }
    private void OnPurchase()
    {
        if (_shopManager != null && currencyItem != null)
        {
            _shopManager.Purchase(currencyItem);
        }
    }
}
