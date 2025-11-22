using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemSO : ScriptableObject
{
    public ShopItemType itemType;
    [Header("Bundle Info")]
    public string bundleName;
    public List<AbilityData> abilities = new List<AbilityData>();
    public List<CurrencyData> currencies = new List<CurrencyData>();
    public int bundlePrice;
    public bool isPurchased;

    [Header("Ability Info")]
    public string abilityName;
    public Sprite abilityIcon;
    [TextArea] public string description;
    public int abilityPrice;

    [Header("Currency Info")]
    public string currencyName;
    public Sprite currencyIcon;
    public int currencyPrice;

    private void OnValidate()
    {
        if (abilities.Count > 4)
        {
            abilities.RemoveRange(4, abilities.Count - 4);
        }

        if (currencies.Count > 3)
        {
            currencies.RemoveRange(3, currencies.Count - 3);
        }
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
    public string abilityName;
    public Sprite abilityIcon;
    public int quantity; 
}

[System.Serializable]
public class CurrencyData
{
    public Sprite currencyIcon;
    public int quantity;
}
