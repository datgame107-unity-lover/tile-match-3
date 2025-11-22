using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject shopBundlePrefab;
    public GameObject shopAbilityPrefab;
    public GameObject abilityItemPrefab;
    public GameObject currencyItemPrefab;

    [Header("Shop Manager")]
    public ShopManager _shopManager;

    [Header("Tabs Buttons")]
    public Button bundleButton;
    public Button abilityButton;
    public Button currencyButton;

    [Header("Shop Views")]
    public GameObject bundleView;
    public GameObject abilityView;
    public GameObject currencyView;

    private GameObject currentShopView;

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
        currentShopView = bundleView;
        ShowShopView(currentShopView);

        bundleButton.onClick.AddListener(() => ShowShopView(bundleView));
        abilityButton.onClick.AddListener(() => ShowShopView(abilityView));
        currencyButton.onClick.AddListener(() => ShowShopView(currencyView));
    }

    private void OnDisable()
    {
        bundleButton.onClick.RemoveAllListeners();
        abilityButton.onClick.RemoveAllListeners();
        currencyButton.onClick.RemoveAllListeners();
    }

    private void LoadShops()
    {
        LoadBundleShop();
        LoadAbilityShop();
        LoadCurrencyShop();
    }

    private void LoadBundleShop()
    {
        Transform content = bundleView.transform.Find("Viewport/Content");
        foreach (var bundle in bundleShopItems)
        {
            Transform bundleItem = Instantiate(shopBundlePrefab, content).transform;

            bundleItem.Find("Container/BundleNameText").GetComponent<TextMeshProUGUI>().text = bundle.bundleName;
            bundleItem.Find("Container/Center/PurchaseButton/PriceText").GetComponent<TextMeshProUGUI>().text = bundle.bundlePrice.ToString();

            Transform abilityParent = bundleItem.Find("Container/Center/AbilityContainer");
            foreach (var ability in bundle.abilities)
            {
                Transform abilityItem = Instantiate(abilityItemPrefab, abilityParent).transform;
                abilityItem.Find("AbilityImage").GetComponent<Image>().sprite = ability.abilityIcon;
                abilityItem.Find("AbilityCountText").GetComponent<TextMeshProUGUI>().text = $"x {ability.quantity}";
            }

            Transform currencyParent = bundleItem.Find("Container/Center/CurrencyContainer");
            foreach (var currency in bundle.currencies)
            {
                Transform currencyItem = Instantiate(currencyItemPrefab, currencyParent).transform;
                currencyItem.Find("CurrencyImage").GetComponent<Image>().sprite = currency.currencyIcon;
                currencyItem.Find("CurrencyText").GetComponent<TextMeshProUGUI>().text = currency.quantity.ToString();
            }
        }
    }

    private void LoadAbilityShop()
    {
        Transform content = abilityView.transform.Find("Viewport/Content");
        foreach (var ability in abilityShopItems)
        {
            Transform abilityItem = Instantiate(shopAbilityPrefab, content).transform;
            abilityItem.Find("Container/Center/AbilityImage").GetComponent<Image>().sprite = ability.abilityIcon;
            abilityItem.Find("Container/Center/AbilityNameText").GetComponent<TextMeshProUGUI>().text = ability.abilityName;
            abilityItem.Find("Container/Center/AbilityDescriptionText").GetComponent<TextMeshProUGUI>().text = ability.description;
            abilityItem.Find("Container/Center/PurchaseButton/PriceText").GetComponent<TextMeshProUGUI>().text = ability.abilityPrice.ToString();
        }
    }

    private void LoadCurrencyShop()
    {
        Transform content = currencyView.transform.Find("Viewport/Content");
        foreach (var currency in currencyShopItems)
        {
            Transform currencyItem = Instantiate(currencyItemPrefab, content).transform;
            currencyItem.Find("CurrencyImage").GetComponent<Image>().sprite = currency.currencyIcon;
        }
    }

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
