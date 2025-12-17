using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemSO : ScriptableObject
{
    [Header("IAP")]
    public string iapProductId;   // bundle1, bundle2, FIRSTPURCHASE

    public ShopItemType itemType;

    [Header("Bundle Info")]
    public string bundleName;
    public List<AbilityData> abilities = new List<AbilityData>();
    public List<CurrencyData> currencies = new List<CurrencyData>();
    public bool isOneTimePurchase;      // phù hợp với editor
    public int bundlePrice;             // nếu bạn muốn xài buyWith thì xoá cái này

    [Header("Single Item Info (Ability + Currency)")]
    public string itemName;             // dùng chung cho Ability / Currency
    public Sprite icon;                 // dùng chung
    [TextArea] public string description;
    public int quantity;                // amount / stack
    public CurrencyType currencyType;
    [Header("Purchase Setting")]
    public ShopCurrencyData buyWith;

    private void OnValidate()
    {
        if (abilities.Count > 4)
            abilities.RemoveRange(4, abilities.Count - 4);

        if (currencies.Count > 3)
            currencies.RemoveRange(3, currencies.Count - 3);
    }
}

public enum ShopItemType
{
    Bundle,
    Ability,
    Currency,
}

[System.Serializable]
public class AbilityData
{
    public CurrencyType abilityType;
    public string abilityName;
    public Sprite abilityIcon;
    public int quantity;
}

[System.Serializable]
public class ShopCurrencyData
{
    public CurrencyType currencyType;
    public string currencyName;
    public Sprite currencyIcon;
    public int price;
}
