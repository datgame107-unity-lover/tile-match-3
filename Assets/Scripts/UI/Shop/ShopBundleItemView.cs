using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBundleItemView : ShopItemView
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Transform rewardRoot;
    [SerializeField] RewardRowUI rewardPrefab;
    [SerializeField] Button buyBtn;
    [SerializeField] TextMeshProUGUI price;

    private ShopItemSO item;
    private ShopService service;

    public override void Bind(ShopItemSO data, ShopService svc)
    {
        item = data;
        service = svc;

        icon.sprite = data.icon;
        title.text = data.itemName;
        price.text = data.buyWith.price.ToString();

        PopulateRewards();

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(Buy);
    }

    void PopulateRewards()
    {
        foreach (Transform c in rewardRoot)
            Destroy(c.gameObject);

        foreach (var c in item.currencies)
        {
            var row = Instantiate(rewardPrefab, rewardRoot);
            row.BindCurrency(c);
        }

        foreach (var a in item.abilities)
        {
            var row = Instantiate(rewardPrefab, rewardRoot);
            row.BindAbility(a);
        }
    }

    void Buy()
    {
        service.GrantIAP(item.iapProductId);
    }
}