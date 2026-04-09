// Scripts/Application/ShopService.cs
using System.Collections.Generic;
using UnityEngine;

public class ShopService
{
    private readonly List<ShopItemSO> items;
    private readonly CurrencyService currency;

    public ShopService(List<ShopItemSO> items, CurrencyService currency)
    {
        this.items = items;
        this.currency = currency;
    }

    // ── Soft currency purchase ────────────────────────
    public bool Purchase(ShopItemSO item)
    {
        if (!currency.Spend(item.buyWith.currencyType, item.buyWith.price))
            return false;

        GrantItem(item);
        return true;
    }

    // ── IAP ───────────────────────────────────────────
    public void GrantIAP(string productId)
    {
        Debug.Log("Ok");
        if (string.IsNullOrEmpty(productId))
        {
            Debug.LogError("[ShopService] productId is null or empty");
            return;
        }

        var bundle = items.Find(i =>
            i.itemType == ShopItemType.Bundle &&
            i.iapProductId == productId);

        if (bundle == null)
        {
            Debug.LogError($"[ShopService] Bundle not found: {productId}");
            return;
        }

        GrantBundle(bundle);
    }

    // ── Query ─────────────────────────────────────────
    public List<ShopItemSO> GetByType(ShopItemType type) =>
        items.FindAll(i => i.itemType == type);

    // ── Private ───────────────────────────────────────
    private void GrantItem(ShopItemSO item)
    {
        currency.Add(item.currencyType, item.quantity);
        EventBus<ItemPurchasedEvent>.Publish(
            new ItemPurchasedEvent { item = item });
    }

    private void GrantBundle(ShopItemSO bundle)
    {
        foreach (var ability in bundle.abilities)
            currency.Add(ability.abilityType, ability.quantity);

        foreach (var c in bundle.currencies)
            currency.Add(c.currencyType, c.quantity);

        EventBus<ItemPurchasedEvent>.Publish(
            new ItemPurchasedEvent { item = bundle });
    }
}