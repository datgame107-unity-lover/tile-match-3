using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{
    public static Action<Tile> OnTileClicked;
    public static Action<TileDataSO> OnTileRemoved;
    
}
