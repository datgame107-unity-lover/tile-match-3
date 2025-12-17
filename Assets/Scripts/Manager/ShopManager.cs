using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;


    [SerializeField]
    private List<ShopItemSO> shopItems;

    private CurrencyManager currencyManager;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currencyManager = CurrencyManager.Instance;
    }

    // ======================
    // SOFT CURRENCY PURCHASE
    // ======================
    public void Purchase(ShopItemSO item)
    {
        if (!currencyManager.Spend(item.buyWith.currencyType, item.buyWith.price))
            return;

        GrantItem(item);
    }

    // ======================
    // IAP GRANT
    // ======================
    public void GrantIAP(string productKey)
    {
        if (string.IsNullOrEmpty(productKey))
        {
            Debug.LogError("GrantIAP: productKey is null or empty");
            return;
        }

        ShopItemSO bundle = shopItems.Find(i =>
            i.itemType == ShopItemType.Bundle &&
            i.iapProductId == productKey
        );

        if (bundle == null)
        {
            Debug.LogError($"GrantIAP failed: Bundle not found for {productKey}");
            return;
        }

        GrantBundle(bundle);
    }

    private void GrantBundle(ShopItemSO bundle)
    {
        if (bundle.itemType != ShopItemType.Bundle)
            return;

        // Grant abilities
        foreach (AbilityData ability in bundle.abilities)
        {
          currencyManager.Add(ability.abilityType, ability.quantity);
        }

        // Grant currencies
        foreach (CurrencyData currency in bundle.currencies)
        {
            currencyManager.Add(currency.currencyType, currency.quantity);

            EventManager.OnCurrencyChanged?.Invoke(
                currency.currencyType,
                currencyManager.Get(currency.currencyType)
            );
        }

        EventManager.OnBoughtItem?.Invoke(bundle);
    }

    // ======================
    // COMMON GRANT LOGIC
    // ======================
    private void GrantItem(ShopItemSO item)
    {
        currencyManager.Add(item.currencyType, item.quantity);

        EventManager.OnCurrencyChanged?.Invoke(
            item.currencyType,
            currencyManager.Get(item.currencyType)
        );

        EventManager.OnBoughtItem?.Invoke(item);
    }

    // ======================
    // QUERY
    // ======================
    public List<ShopItemSO> GetItemsByType(ShopItemType type)
    {
        return shopItems.FindAll(i => i.itemType == type);
    }
}
