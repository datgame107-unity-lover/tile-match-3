using System.Collections.Generic;
using UnityEngine;

public class SortingLayerOrder : MonoBehaviour
{
    [SerializeField] private Transform grid;

    private void LateUpdate()
    {
        SortTiles();
        ActivateShadows(); 
    }

    private void SortTiles()
    {
        foreach (Transform child in grid)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile == null) continue;

            int baseOrder = tile.layer * 10;

            foreach (var renderer in child.GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.name == "Shadow")
                    renderer.sortingOrder = baseOrder +2;
                else if (renderer.name == "Base")
                    renderer.sortingOrder = baseOrder;
                else if (renderer.name == "Food")
                    renderer.sortingOrder = baseOrder + 1;
                else
                    renderer.sortingOrder = baseOrder; // default
            }
        }
    }
    private void ActivateShadows()
    {
        foreach (Transform child in grid)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile == null) continue;

            Collider2D tileCollider = tile.GetComponent<Collider2D>();
            if (tileCollider == null) continue;

            // Tạo danh sách để lưu collider overlap
            List<Collider2D> results = new List<Collider2D>();
             ContactFilter2D filter = new ContactFilter2D().NoFilter(); // không lọc gì cả

            int hitCount = tileCollider.Overlap(filter, results);

            // Duyệt qua các Tile đè lên Tile hiện tại
            bool showShadow = false;

            foreach (var hit in results)
            {
                Tile otherTile = hit.GetComponent<Tile>();
                if (otherTile != null && otherTile.layer > tile.layer)
                {
                    showShadow = true;
                    break;
                }
            }
            if(!showShadow)
            {
                tile.isBlocked = false;
            }  else
            {
                tile.isBlocked = true;
            }    
            // Bật hoặc tắt shadow
            var shadow = tile.transform.Find("Shadow")?.GetComponent<SpriteRenderer>();
            if (shadow != null)
                shadow.enabled = showShadow;
        }
    }

}
