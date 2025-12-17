using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Prefabs")]
    public ShopBundleUI shopBundlePrefab;
    public ShopAbilityUI shopAbilityPrefab;
    //public ShopCurrencyUI shopCurrencyPrefab;

    [Header("Shop Manager")]
    public ShopManager _shopManager;

    [Header("Tabs Buttons")]
    public Button bundleButton;
    public Button abilityButton;
    public Button currencyButton;
    public Button shopOverlay;

    [Header("Shop Views")]
    public GameObject bundleView;
    public GameObject abilityView;
    //public GameObject currencyView;
    private GameObject currentShopView;

    [Header("Transaction Anouncement")]
    public GameObject transactionPanel;


    private List<ShopItemSO> bundleShopItems;
    private List<ShopItemSO> abilityShopItems;
    private List<ShopItemSO> currencyShopItems;

    private void Start()
    {
        bundleShopItems = _shopManager.GetItemsByType(ShopItemType.Bundle);
        abilityShopItems = _shopManager.GetItemsByType(ShopItemType.Ability);
        currencyShopItems = _shopManager.GetItemsByType(ShopItemType.Currency);

        LoadShops();
    }

    private void OnEnable()
    {
        EventManager.OnTransactionComplete += HandleTransactionComplete;

        currentShopView = bundleView;
        ShowShopView(currentShopView);

        bundleButton.onClick.AddListener(() => ShowShopView(bundleView));
        abilityButton.onClick.AddListener(() => ShowShopView(abilityView));
        //currencyButton.onClick.AddListener(() => ShowShopView(currencyView));
        
        shopOverlay.onClick.AddListener(() =>
        {
            CloseShop();
        });
    }

    private void OnDisable()
    {
        EventManager.OnTransactionComplete -= HandleTransactionComplete;


        bundleButton.onClick.RemoveAllListeners();
        abilityButton.onClick.RemoveAllListeners();
        currencyButton.onClick.RemoveAllListeners();
    }

    private void LoadShops()
    {
        LoadBundleShop();
        LoadAbilityShop();
        //LoadCurrencyShop();
    }

    private void HandleTransactionComplete()
    {
        GameObject trans = Instantiate(transactionPanel,transform);
        Destroy(trans, 1.5f);
    }
    private void LoadBundleShop()
    {
        Transform content = bundleView.transform.Find("Viewport/Content");
        foreach (var bundle in bundleShopItems)
        {
           ShopBundleUI bundleItem = Instantiate(shopBundlePrefab, content).GetComponent<ShopBundleUI>();
            bundleItem.SetupBundle(bundle);
        }
    }

    private void LoadAbilityShop()
    {
        Transform content = abilityView.transform.Find("Viewport/Content");
        foreach (var ability in abilityShopItems)
        {
            ShopAbilityUI abilityItem = Instantiate(shopAbilityPrefab, content).GetComponent<ShopAbilityUI>();
            abilityItem.SetupAbility(ability);
        }
    }

    //private void LoadCurrencyShop()
    //{
    //    Transform content = currencyView.transform.Find("Viewport/Content");
    //    foreach (var currency in currencyShopItems)
    //    {
    //        ShopCurrencyUI currencyItem = Instantiate(shopCurrencyPrefab, content).GetComponent<ShopCurrencyUI>();
    //        currencyItem.SetupCurrency(currency);
    //    }
    //}

    private void ShowShopView(GameObject viewToShow)
    {
        if (currentShopView != null)
            currentShopView.SetActive(false);

        viewToShow.SetActive(true);
        currentShopView = viewToShow;
    }

    public void OpenShop()
    {
        this.gameObject.SetActive(true);
        ShowShopView(bundleView); 
    }

    public void CloseShop()
    {
        this.gameObject.SetActive(false);
    }
}
