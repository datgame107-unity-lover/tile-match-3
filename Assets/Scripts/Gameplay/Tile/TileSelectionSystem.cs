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

        if (context.SelectingTiles.Count >= context.MaxSelectableTile) return;

        tile.isClicked = true;

        // Xóa gạch khỏi bàn chơi
        context.CurrentTiles.Remove(tile);
        tile.gameObject.SetActive(false);

        var levelBuilder = ServiceLocator.Get<LevelBuilder>();
        var gridConfig = ServiceLocator.Get<GridConfig>();

        if (levelBuilder != null && gridConfig != null)
        {
            levelBuilder.RefreshAllBlocking(context.CurrentTiles, gridConfig.colliderSize);
        }
        // ----------------------------------------------

        int insertIndex = context.SelectingTiles.Count;
        for (int i = context.SelectingTiles.Count - 1; i >= 0; i--)
        {
            if (context.SelectingTiles[i].tileData == tile.tileData)
            {
                insertIndex = i + 1;
                break;
            }
        }

        context.SelectingTiles.Insert(insertIndex, tile);

        EventBus<TileSelectedEvent>.Publish(
            new TileSelectedEvent { tile = tile });
    }
}