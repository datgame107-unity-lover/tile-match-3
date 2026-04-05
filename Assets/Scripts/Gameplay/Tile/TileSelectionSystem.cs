// Scripts/Gameplay/Tile/TileSelectionSystem.cs
using System.Collections.Generic;

public class TileSelectionSystem
{
    private readonly BoardContext context;

    public TileSelectionSystem(BoardContext context)
    {
        this.context = context;
    }

    public void Select(Tile tile)
    {
        if (tile == null || tile.isClicked) return;

        if (context.SelectingTiles.Count >= context.MaxSelectableTile)
        {
            ServiceLocator.Get<GameStateService>()
                          .ChangeState(GameState.Lose);
            return;
        }

        tile.isClicked = true;
        context.SelectingTiles.Add(tile);
        context.CurrentTiles.Remove(tile);
        tile.gameObject.SetActive(false);

        EventBus<TileSelectedEvent>.Publish(
            new TileSelectedEvent { tile = tile });
    }
}