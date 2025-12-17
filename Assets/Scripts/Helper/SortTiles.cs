using System.Collections.Generic;
using UnityEngine;

public static class SortTiles 
{
    public static void Sort(List<Tile> tiles)
    {
        if (tiles == null) return;

        foreach (Tile tile in tiles)
        {
            if (tile == null) continue;

            int baseOrder = tile.layer * 10;

            var container = tile.transform.Find("Container");
            if (container == null) continue;

            foreach (var renderer in container.GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer == null) continue;

                if (renderer.name == "Shadow")
                    renderer.sortingOrder = baseOrder + 2;
                else if (renderer.name == "Base")
                    renderer.sortingOrder = baseOrder;
                else if (renderer.name == "Food")
                    renderer.sortingOrder = baseOrder + 1;
                else
                    renderer.sortingOrder = baseOrder;
            }
        }
    }

    public static void ActivateShadows(List<Tile> tiles)
    {   
        if (tiles == null) return;
        foreach (Tile tile in tiles)
        {
            if (tile == null) continue;

            Collider2D tileCollider = tile.GetComponent<Collider2D>();
            if (tileCollider == null) continue;

            // Tạo box trung tâm 85% của tile
            Vector2 center = tileCollider.bounds.center;
            Vector2 size = tileCollider.bounds.size * 0.85f;

            // Kiểm tra overlap
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
            bool showShadow = false;

            foreach (var hit in hits)
            {
                if (hit == null) continue;
                Tile otherTile = hit.GetComponent<Tile>();
                if (otherTile != null && otherTile.layer > tile.layer)
                {
                    showShadow = true;
                    break;
                }
            }

            tile.isBlocked = showShadow;

            var shadow = tile.transform.Find("Container/Shadow");
            if (shadow != null)
                shadow.gameObject.SetActive(showShadow);
        }
    }

}
