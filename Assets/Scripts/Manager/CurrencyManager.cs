using UnityEngine;
using System.Collections.Generic;
using System;

public enum CurrencyType
{
    Flower,
    Diamond,
    Heart,

    Hint,
    Shuffle,
    PowerUp,
    Undo
}

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    private Dictionary<CurrencyType, int> currencies = new Dictionary<CurrencyType, int>();
    private CurrencyType[] currencyTypes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));

            InitCurrencies();
            LoadAll();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitCurrencies()
    {
        foreach (CurrencyType type in currencyTypes)
        {
            currencies[type] = 0;
        }

        // Default heart = 5
        currencies[CurrencyType.Heart] = 5;
    }

    private void LoadAll()
    {
        foreach (CurrencyType type in currencyTypes)
        {
            currencies[type] = PlayerPrefs.GetInt(type.ToString(), currencies[type]);
        }
    }

    private void SaveAll()
    {
        foreach (CurrencyType type in currencyTypes)
        {
            PlayerPrefs.SetInt(type.ToString(), currencies[type]);
        }

        PlayerPrefs.Save();
    }

    public int Get(CurrencyType type)
    {
        return currencies[type];
    }

    public void Add(CurrencyType type, int amount)
    {
        currencies[type] += amount;

        SaveAll();
        EventManager.OnCurrencyChanged?.Invoke(type, currencies[type]);
    }

    public bool Spend(CurrencyType type, int amount)
    {
        if (currencies[type] < amount)
            return false;

        currencies[type] -= amount;

        SaveAll();
        EventManager.OnCurrencyChanged?.Invoke(type, currencies[type]);
        return true;
    }
}
