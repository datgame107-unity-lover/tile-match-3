using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSingleItemView : ShopItemView
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI amount;
    [SerializeField] Button buyBtn;
    [SerializeField] TextMeshProUGUI price;

    private ShopItemSO item;
    private ShopService service;

    public override void Bind(ShopItemSO data, ShopService svc)
    {
        item = data;
        service = svc;

        icon.sprite = data.icon;
        amount.text = $"+{data.quantity}";
        price.text = data.buyWith.price.ToString();

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(Buy);
    }

    void Buy()
    {
        service.Purchase(item);
    }
}