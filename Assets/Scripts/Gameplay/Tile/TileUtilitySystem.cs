// Scripts/Gameplay/Tile/TileUtilitySystem.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileUtilitySystem
{
    private readonly BoardContext context;
    private readonly Stack<List<(Tile tile, int index)>> undoStack = new();

    public TileUtilitySystem(BoardContext context)
    {
        this.context = context;
    }

    public void Shuffle()
    {
        var currency = ServiceLocator.Get<CurrencyService>();
        if (!currency.CanAfford(CurrencyType.Shuffle, 1)) return;

        var tiles = context.CurrentTiles;
        var dataList = tiles.Select(t => t.tileData).ToList();

        for (int i = dataList.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (dataList[i], dataList[rand]) = (dataList[rand], dataList[i]);
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].tileData = dataList[i];
            tiles[i].ApplyData();
        }

        EventBus<ShuffleUsedEvent>.Publish(new ShuffleUsedEvent());
    }

    public void Hint()
    {
        var currency = ServiceLocator.Get<CurrencyService>();
        if (!currency.CanAfford(CurrencyType.Hint, 1)) return;

        var group = context.CurrentTiles
            .Where(t => t != null && !t.isBlocked)
            .GroupBy(t => t.tileData)
            .FirstOrDefault(g => g.Count() >= 3);

        if (group == null) return;

        foreach (var tile in group.Take(3))
        {
            var container = tile.transform.Find("Container");
            if (container != null)
            {
                //DOAnimationManager.ScaleBounce(
                //    container, tile.GetOriginalScale(), 1.3f, 0.1f);

            }
        }

        EventBus<HintUsedEvent>.Publish(new HintUsedEvent());
    }

    public void PowerUp()
    {
        var currency = ServiceLocator.Get<CurrencyService>();
        if (!currency.CanAfford(CurrencyType.PowerUp, 1)) return;

        var group = context.CurrentTiles
            .Where(t => t != null)
            .GroupBy(t => t.tileData)
            .FirstOrDefault(g => g.Count() >= 3);

        if (group == null) return;

        var toRemove = group.Take(3).ToList();
        foreach (var tile in toRemove)
        {
            context.CurrentTiles.Remove(tile);
            Object.Destroy(tile.gameObject);
        }

        EventBus<PowerUpUsedEvent>.Publish(new PowerUpUsedEvent());
        EventBus<TilesRemovedEvent>.Publish(
            new TilesRemovedEvent { tileData = toRemove[0].tileData });
    }

    public void PushUndoSnapshot(Tile tile, int slotIndex)
    {
        undoStack.Push(new List<(Tile, int)> { (tile, slotIndex) });
    }

    public void Undo()
    {
        var currency = ServiceLocator.Get<CurrencyService>();
        if (!currency.CanAfford(CurrencyType.Undo, 1)) return;
        if (undoStack.Count == 0) return;

        var snapshot = undoStack.Pop();
        foreach (var (tile, _) in snapshot)
        {
            if (tile == null) continue;

            context.SelectingTiles.Remove(tile);
            context.CurrentTiles.Add(tile);

            tile.isClicked = false;
            tile.gameObject.SetActive(true);
        }

        EventBus<UndoUsedEvent>.Publish(new UndoUsedEvent());
    }
}