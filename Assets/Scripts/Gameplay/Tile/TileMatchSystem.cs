// Scripts/Gameplay/Tile/TileMatchSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class TileMatchSystem
{
    private readonly BoardContext context;

    public TileMatchSystem(BoardContext context)
    {
        this.context = context;
    }

    public void CheckMatch()
    {
        var selecting = context.SelectingTiles;
        bool hasMatch = false;

        if (selecting.Count >= 3)
        {
            for (int i = 0; i <= selecting.Count - 3; i++)
            {
                Tile a = selecting[i];
                Tile b = selecting[i + 1];
                Tile c = selecting[i + 2];

                if (a.tileData == b.tileData && a.tileData == c.tileData)
                {
                    RemoveMatch(a, b, c);
                    hasMatch = true;
                    break; // Thoát lặp, chỉ xử lý 1 match mỗi lần
                }
            }
        }

        if (!hasMatch && selecting.Count >= context.MaxSelectableTile)
        {
            ServiceLocator.Get<GameStateService>().ChangeState(GameState.Lose);
        }
    }

    private void RemoveMatch(Tile a, Tile b, Tile c)
    {
        TileDataSO matchedData = a.tileData;

        context.SelectingTiles.Remove(a);
        context.SelectingTiles.Remove(b);
        context.SelectingTiles.Remove(c);

        if (a != null) Object.Destroy(a.gameObject);
        if (b != null) Object.Destroy(b.gameObject);
        if (c != null) Object.Destroy(c.gameObject);

        EventBus<TilesRemovedEvent>.Publish(
            new TilesRemovedEvent { tileData = matchedData });
    }
}