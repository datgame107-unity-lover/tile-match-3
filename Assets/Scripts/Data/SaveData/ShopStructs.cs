// Scripts/Data/SaveData/ShopStructs.cs
using System;
using UnityEngine;

[Serializable]
public class PriceData
{
    public CurrencyType currencyType;
    public int price;
}

[Serializable]
public class QuestReward
{
    public CurrencyType type;
    public int amount;
}

[Serializable]
public class AbilityData
{
    public CurrencyType abilityType;
    public Sprite icon;

    public int quantity;
}

[Serializable]
public class CurrencyData
{
    public CurrencyType currencyType;
    public Sprite icon;
    public int quantity;
}