using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Reward
{
    public CurrencyType rewardType;
    public int amount;
}
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public QuestReward[] winRewards;

    [Header("Heart Regen")]
    public int maxHeart = 5;
    public float heartRegenTime = 30f; // 5 phút

    private float heartTimer;
    private const string HEART_TIMER_KEY = "HeartTimer";


    private Dictionary<CurrencyType, int> currencies = new Dictionary<CurrencyType, int>();
    private CurrencyType[] currencyTypes;

    private Action shuffleAction;
    private Action undoAction;
    private Action hintAction;
    private Action powerUpAction;

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

    private void OnEnable()
    {
        EventManager.OnPlayerWon += HandlePlayerWin;
        EventManager.OnPlayerLost += HandlePlayerLost;

        shuffleAction = () => HandleAbilityUsed(CurrencyType.Shuffle);
        undoAction = () => HandleAbilityUsed(CurrencyType.Undo);
        hintAction = () => HandleAbilityUsed(CurrencyType.Hint);
        powerUpAction = () => HandleAbilityUsed(CurrencyType.PowerUp);

        EventManager.OnShuffleUsed += shuffleAction;
        EventManager.OnUndoUsed += undoAction;
        EventManager.OnHintUsed += hintAction;
        EventManager.OnPowerUpUsed += powerUpAction;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerWon -= HandlePlayerWin;
        EventManager.OnPlayerLost -= HandlePlayerLost;

        EventManager.OnShuffleUsed -= shuffleAction;
        EventManager.OnUndoUsed -= undoAction;
        EventManager.OnHintUsed -= hintAction;
        EventManager.OnPowerUpUsed -= powerUpAction;
    }

        private void HandleAbilityUsed(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Shuffle:
                Spend(CurrencyType.Shuffle,1);
                break;
            case CurrencyType.Undo:
                Spend(CurrencyType.Undo,1);

                break;
            case CurrencyType.Hint:
                Spend(CurrencyType.Hint, 1);

                break;
            case CurrencyType.PowerUp:
                Spend(CurrencyType.PowerUp, 1);

                break;
        }
    }

private void Update()
    {
        if (currencies[CurrencyType.Heart] >= maxHeart) return;

        heartTimer += Time.deltaTime;

        if (heartTimer >= heartRegenTime)
        {
            heartTimer = 0;
            Add(CurrencyType.Heart, 1);
        }
    }
    
    private void HandlePlayerWin()
    {
        AddWinReward(winRewards);
    }
    private void HandlePlayerLost()
    {
        Spend(CurrencyType.Heart, 1);
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
        heartTimer = PlayerPrefs.GetFloat(HEART_TIMER_KEY, 0);
        currencies[CurrencyType.Heart] = 0;

    }


    private void SaveAll()
    {
        foreach (CurrencyType type in currencyTypes)
        {
            PlayerPrefs.SetInt(type.ToString(), currencies[type]);
        }

        PlayerPrefs.SetFloat(HEART_TIMER_KEY, heartTimer);
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
        if (type == CurrencyType.Heart && currencies[type] <= 0)
            return false;

        if (currencies[type] < amount)
            return false;

        currencies[type] -= amount;

        if (type == CurrencyType.Heart && currencies[type] < maxHeart)
        {
            heartTimer = 0; // reset timer khi vừa mất heart
        }

        SaveAll();
        EventManager.OnCurrencyChanged?.Invoke(type, currencies[type]);
        return true;
    }

    public void AddWinReward(QuestReward[] rewards)
    {
        foreach (QuestReward reward in rewards)
        {

            Add(reward.type, reward.amount);
        }
    }
    public int GetWinRewards(CurrencyType type)
    {
        foreach (QuestReward reward in winRewards)
        {
            if (reward.type == type) { return reward.amount; }
        }
        return 0;
    }
    public void GrantBundle(ShopItemSO bundle)
    {
        if (bundle == null || bundle.itemType != ShopItemType.Bundle)
            return;

        // Grant abilities
        foreach (var ability in bundle.abilities)
        {
           Add(ability.abilityType, ability.quantity);
        }

        // Grant currencies + FX
        foreach (var currency in bundle.currencies)
        {
           Add(currency.currencyType, currency.quantity);

            EventManager.OnCurrencyChanged?.Invoke(
                currency.currencyType,
              Get(currency.currencyType)
            );

         
        }

        EventManager.OnBoughtItem?.Invoke(bundle);
    }
    public float GetHeartRemainTime()
    {
        if (currencies[CurrencyType.Heart] >= maxHeart) return 0;
        return heartRegenTime - heartTimer;
    }

}
