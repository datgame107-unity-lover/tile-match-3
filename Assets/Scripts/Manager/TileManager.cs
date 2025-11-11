using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class TileManager : MonoBehaviour
{
    public int maxSeletableTile = 8;
    List<Tile> currentTiles;
    List<Tile> selectingTiles;
    private Tile currentHoveredTile;
    private void Start()
    {
        print(PlayerPrefs.GetInt("level"));
        GenerateNewLevel(PlayerPrefs.GetInt("level"));
        currentTiles = GetComponentsInChildren<Tile>().ToList();
        selectingTiles = new List<Tile>();

    }
    private void LateUpdate()
    {
        SortTiles();
        ActivateShadows();
    }
    private void OnEnable()
    {
        EventManager.OnContinueButtonClicked += HandleContinueLevel;
    }

    private void OnDisable()
    {
        EventManager.OnContinueButtonClicked -= HandleContinueLevel;
    }

    private void HandleContinueLevel()
    {
        GenerateNewLevel(PlayerPrefs.GetInt("level"));
    }
    private void Update()
    {
        if (GameManager.instance.currentState != GameState.Playing) return;

        Tile tile = GetTopTileUnderMouse();

        if (Input.GetMouseButton(0))
        {
            if (tile != currentHoveredTile)
            {
                // Nếu có tile cũ, nhỏ lại
                if (currentHoveredTile != null)
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform, 1f); // trở về scale 1

                currentHoveredTile = tile;

                // Nếu tile mới hợp lệ, scale bự
                if (currentHoveredTile != null && !currentHoveredTile.isBlocked)
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform, 1.2f);
            }

            // Nếu không còn tile dưới chuột, nhỏ lại tile hiện tại
            if (tile == null && currentHoveredTile != null)
            {
                DOAnimationManager.ScaleBounce(currentHoveredTile.transform, 1f);
                currentHoveredTile = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentHoveredTile != null && !currentHoveredTile.isBlocked)
            {
                SelectTile(currentHoveredTile);
                DOAnimationManager.ScaleBounce(currentHoveredTile.transform, 1f); // nhỏ lại
                currentHoveredTile = null;
            }
        }
    }

    private Tile GetTopTileUnderMouse()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        int highestLayer = int.MinValue;
        Tile topTile = null;

        foreach (RaycastHit2D hit in hits)
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile == null) continue;

            if (tile.layer >= highestLayer)
            {
                highestLayer = tile.layer;
                topTile = tile;
            }
        }

        return topTile;
    }

    private void SelectTile(Tile tile)
    {
        if (selectingTiles.Count >= maxSeletableTile)
        {
            GameManager.instance.ChangeState(GameState.Lose);
            return;
        }
        selectingTiles.Add(tile);
        print("hehe");
        tile.gameObject.SetActive(false);
        for (int i = selectingTiles.Count - 2; i >= 0; i--)
        {
            if (selectingTiles[i].tileData == tile.tileData)
            {
                for (int j = selectingTiles.Count - 1; j > i + 1; j--)
                {
                    (selectingTiles[j], selectingTiles[j - 1]) = (selectingTiles[j - 1], selectingTiles[j]);
                }
                break;
            }

        }
        EventManager.OnTileSelected?.Invoke(tile);
        CheckMatch3Condition(tile.tileData);
    }

    private void CheckMatch3Condition(TileDataSO tileDataSO)
    {
        if (selectingTiles.Count < 3)
            return;

        for (int i = 0; i < selectingTiles.Count - 2; i++)
        {
            var t1 = selectingTiles[i];
            var t2 = selectingTiles[i + 1];
            var t3 = selectingTiles[i + 2];

            if (t1.tileData == t2.tileData && t1.tileData == t3.tileData)
            {
                List<Tile> tilesToRemove = new List<Tile> { t1, t2, t3 };

                foreach (var tile in tilesToRemove)
                {
                    currentTiles.Remove(tile);
                    selectingTiles.Remove(tile);
                    Destroy(tile.gameObject);
                }

                // Gọi sự kiện
                EventManager.OnTileRemoved?.Invoke(tileDataSO);
                break; // Dừng sau khi xoá match đầu tiên
            }
        }
        if (currentTiles.Count == 0 && selectingTiles.Count == 0)
        {
            StartCoroutine(Win());
        }
    }

    private void GenerateNewLevel(int level)
    {
        LevelManager.Instance.LoadFromSO(level);
        currentTiles = GetComponentsInChildren<Tile>().ToList();
        GameManager.instance.ChangeState(GameState.Playing);
    }


    private IEnumerator Win()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.ChangeState(GameState.Win);

    }

    private void SortTiles()
    {
        foreach (Tile tile in currentTiles)
        {
            if (tile == null) continue;

            int baseOrder = tile.layer * 10;

            foreach (var renderer in tile.GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.name == "Shadow")
                    renderer.sortingOrder = baseOrder + 2;
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

        foreach (Tile tile in currentTiles)
        {
            if (tile == null) continue;

            Collider2D tileCollider = tile.GetComponent<Collider2D>();
            if (tileCollider == null) continue;

            List<Collider2D> results = new List<Collider2D>();

            int hitCount = tileCollider.Overlap(ContactFilter2D.noFilter, results);

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
            if (!showShadow)
            {
                tile.isBlocked = false;
            }
            else
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
