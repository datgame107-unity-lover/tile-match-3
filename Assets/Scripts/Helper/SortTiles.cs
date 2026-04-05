// Scripts/Helper/SortTiles.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SortTiles
{
    public static void Sort(List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0) return;

        // Group theo vị trí (snap về grid 0.1f)
        var groups = tiles.GroupBy(t =>
        {
            var p = t.transform.position;
            return new Vector2(
                Mathf.Round(p.x * 10f) / 10f,
                Mathf.Round(p.y * 10f) / 10f);
        });

        foreach (var group in groups)
        {
            var stack = group.OrderBy(t => t.layer).ToList();
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].layer = i;
                stack[i].UpdateSortingOrder();
            }
        }
    }

    public static void ActivateShadows(List<Tile> tiles)
    {
        if (tiles == null) return;

        // Tile bị chặn = có tile khác cùng vị trí với layer cao hơn
        var positions = tiles
            .GroupBy(t =>
            {
                var p = t.transform.position;
                return new Vector2(
                    Mathf.Round(p.x * 10f) / 10f,
                    Mathf.Round(p.y * 10f) / 10f);
            })
            .Where(g => g.Count() > 1);

        // Reset tất cả
        foreach (var tile in tiles)
            tile.SetBlocked(false);

        // Đánh dấu blocked
        foreach (var group in positions)
        {
            var stack = group.OrderBy(t => t.layer).ToList();
            for (int i = 0; i < stack.Count - 1; i++)
                stack[i].SetBlocked(true);
        }
    }
}