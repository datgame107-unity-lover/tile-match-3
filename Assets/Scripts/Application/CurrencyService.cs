// Scripts/Application/CurrencyService.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyService
{
    public int MaxHeart { get; }
    public float HeartRegenTime { get; }

    private readonly ISaveService save;
    private readonly Dictionary<CurrencyType, int> currencies = new();
    private readonly CurrencyType[] allTypes;
    private float heartTimer;

    public CurrencyService(ISaveService save, int maxHeart = 5, float heartRegenTime = 30f)
    {
        this.save = save;
        MaxHeart = maxHeart;
        HeartRegenTime = heartRegenTime;
        allTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));

        InitDefaults();
        Load();
    }

    // ?? Tick — g?i t? GameBootstrapper.Update() ??????
    public void Tick(float deltaTime)
    {
        if (currencies[CurrencyType.Heart] >= MaxHeart) return;

        heartTimer += deltaTime;
        if (heartTimer >= HeartRegenTime)
        {
            heartTimer = 0f;
            Add(CurrencyType.Heart, 1);
        }
    }

    // ?? Query ?????????????????????????????????????????
    public int Get(CurrencyType type)
    {
        currencies.TryGetValue(type, out int val);
        return val;
    }

    public bool CanAfford(CurrencyType type, int amount) =>
        Get(type) >= amount;

    public float GetHeartRemainTime()
    {
        if (currencies[CurrencyType.Heart] >= MaxHeart) return 0f;
        return HeartRegenTime - heartTimer;
    }

    // ?? Mutate ????????????????????????????????????????
    public void Add(CurrencyType type, int amount)
    {
        if (amount <= 0) return;

        currencies[type] = type == CurrencyType.Heart
            ? Mathf.Min(currencies[type] + amount, MaxHeart)
            : currencies[type] + amount;

        SaveAll();
        Publish(type);
    }

    public bool Spend(CurrencyType type, int amount)
    {
        if (!CanAfford(type, amount)) return false;

        currencies[type] -= amount;

        if (type == CurrencyType.Heart)
            heartTimer = 0f;

        SaveAll();

        EventBus<CurrencySpentEvent>.Publish(
            new CurrencySpentEvent { type = type, amount = amount });
        Publish(type);
        return true;
    }

    public void AddBundle(QuestReward[] rewards)
    {
        foreach (var r in rewards)
            Add(r.type, r.amount);
    }

    public int GetRewardAmount(CurrencyType type, QuestReward[] rewards)
    {
        foreach (var r in rewards)
            if (r.type == type) return r.amount;
        return 0;
    }

    // ?? Private ???????????????????????????????????????
    private void Publish(CurrencyType type)
    {
        EventBus<CurrencyChangedEvent>.Publish(
            new CurrencyChangedEvent { type = type, amount = currencies[type] });
    }

    private void InitDefaults()
    {
        foreach (var t in allTypes)
            currencies[t] = t == CurrencyType.Heart ? MaxHeart : 0;
    }

    private void Load()
    {
        foreach (var t in allTypes)
            currencies[t] = save.GetInt(SaveKeys.Currency.Key(t), currencies[t]);

        heartTimer = save.GetFloat(SaveKeys.Heart.RegenTimer, 0f);
    }

    private void SaveAll()
    {
        foreach (var t in allTypes)
            save.SetInt(SaveKeys.Currency.Key(t), currencies[t]);

        save.SetFloat(SaveKeys.Heart.RegenTimer, heartTimer);
        save.Save();
    }
}