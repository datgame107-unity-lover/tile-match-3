using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class EventManager 
{
    public static Action<Tile> OnTileSelected;
    public static Action<TileDataSO> OnTilesRemoved;
    public static Action OnPlayerWon;
    public static Action OnPlayerLost;
    public static Action OnContinueButtonClicked;
    public static Action OnHomeButtonClicked;
    public static Action OnSettingButtonClicked;
    public static Action OnNewGameButtonClicked;
    public static Action<GameState> OnStateChanged;
    public static Action OnSavingNewLevel;
    public static Action OnSavedNewLevel;
    public static Action<TileDataSO> OnTileRemoved;
    public static Action<int> OnChoseLevel;
    public static Action OnCreatingNewLevel;
    public static Action OnRestartLevel;
    public static Action OnPlayOn;
    public static Action<CurrencyType, int> OnCurrencyChanged;

}
