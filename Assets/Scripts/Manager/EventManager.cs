using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{
    public static Action<Tile> OnTileSelected;
    public static Action<TileDataSO> OnTileRemoved;
    public static Action OnPlayerWon;
    public static Action OnPlayerLost;
    public static Action OnContinueButtonClicked;
    public static Action OnHomeButtonClicked;
    public static Action OnSettingButtonClicked;
    public static Action OnNewGameButtonClicked;
    public static Action<GameState> OnStateChanged;
    
}
