// Scripts/Data/SO/ShopItemSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Game/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    [Header("Identity")]
    public string itemName;
    public Sprite icon;
    public ShopItemType itemType;

    [Header("IAP (Bundle only)")]
    public string iapProductId;

    [Header("Soft purchase")]
    public PriceData buyWith;

    [Header("Grant on purchase")]
    public CurrencyType currencyType;   // single currency item
    public int quantity;       // single currency item
    public List<AbilityData> abilities;      // bundle
    public List<CurrencyData> currencies;    // bundle
}