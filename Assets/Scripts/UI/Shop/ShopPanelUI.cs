using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ShopSingleItemView singlePrefab;
    [SerializeField] private ShopBundleItemView bundlePrefab;
    [SerializeField] private Transform contentRoot;

    [Header("Tabs")]
    [SerializeField] private Button currencyTab;
    [SerializeField] private Button bundleTab;

    private ShopService shopService;
    private ShopItemType currentType;
    private readonly List<ShopItemView> views = new();

    private void OnEnable()
    {   
        shopService = ServiceLocator.Get<ShopService>();
        Initialize(shopService);
    }
    public void Initialize(ShopService service)
    {
        shopService = service;

        currencyTab.onClick.AddListener(() =>
            ShowCategory(ShopItemType.Currency));


        bundleTab.onClick.AddListener(() =>
            ShowCategory(ShopItemType.Bundle));

        ShowCategory(ShopItemType.Currency);
    }

    private void ShowCategory(ShopItemType type)
    {
        currentType = type;
        Refresh();
    }

    public void Refresh()
    {
        Clear();

        var items = shopService.GetByType(currentType);
        Spawn(items);
    }

    private void Spawn(List<ShopItemSO> items)
    {
        foreach (var item in items)
        {
            ShopItemView view =
                item.itemType == ShopItemType.Bundle
                ? Instantiate(bundlePrefab, contentRoot)
                : Instantiate(singlePrefab, contentRoot);

            view.Bind(item, shopService);
            views.Add(view);
        }
    }

    private void Clear()
    {
        foreach (var v in views)
            Destroy(v.gameObject);

        views.Clear();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}