using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Settings")]
    public int maxSeletableTile = 9;

    [HideInInspector] public List<Tile> currentTiles;
    [HideInInspector] public List<Tile> selectingTiles;
    public AudioClip clickSoundClip;
    private Tile currentHoveredTile;
    private IGameModeHandler modeHandler;

    private void Start()
    {
        print($"Current Level: {PlayerPrefs.GetInt("level")}");
        GenerateNewGame(GameManager.instance.gameMode);

        currentTiles = GetComponentsInChildren<Tile>().ToList();
        selectingTiles = new List<Tile>();

        // Gán handler theo mode hiện tại
        switch (GameManager.instance.gameMode)
        {
            case GameMode.Level:
                modeHandler = new LevelModeHandler(this);
                break;
            case GameMode.Infinite:
                modeHandler = new InfiniteModeHandler(this);
                break;
        }
    }

    private void OnEnable()
    {
        EventManager.OnContinueButtonClicked += HandleContinueLevel;
    }

    private void OnDisable()
    {
        EventManager.OnContinueButtonClicked -= HandleContinueLevel;
    }

    private void LateUpdate()
    {
        SortTiles();
        ActivateShadows();
    }

    private void Update()
    {
        if (GameManager.instance.currentState != GameState.Playing) return;

        Tile tile = GetTopTileUnderMouse();

        if (Input.GetMouseButton(0))
        {
            if (tile != currentHoveredTile)
            {
                // Nhỏ lại tile cũ
                if (currentHoveredTile != null)
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container").transform, 1f);

                currentHoveredTile = tile;

                // Phóng to tile mới
                if (currentHoveredTile != null && !currentHoveredTile.isBlocked)
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container").transform.transform, 1.2f);
            }

            if (tile == null && currentHoveredTile != null)
            {
                DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container").transform.transform, 1f);
                currentHoveredTile = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentHoveredTile != null && !currentHoveredTile.isBlocked)
            {
                SelectTile(currentHoveredTile);
                DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container").transform.transform, 1f);
                currentHoveredTile = null;
            }
        }
    }

    private void HandleContinueLevel()
    {
        GenerateNewGame(GameMode.Level);
    }

    // 🔹 Sinh map mới tùy theo mode
    public void GenerateNewGame(GameMode gameMode)
    {
        switch (gameMode)
        {
            default:
            case GameMode.Level:
                GenerateNewLevel(GameManager.instance.level);
                break;
            case GameMode.Infinite:
                GenerateInfiniteTile(4);
                break;
        }
    }

    private void GenerateInfiniteTile(int maxLayer)
    {
        for (int i = 0; i < maxLayer - 1; i++)
        {
            LevelManager.Instance.GenerateOneLayer(i, 21);

            SortTiles();
            ActivateShadows();
        }
    }

    private void GenerateNewLevel(int level)
    {
        //LevelManager.Instance.LoadFromSO(level);
        currentTiles = GetComponentsInChildren<Tile>().ToList();
        GameManager.instance.ChangeState(GameState.Playing);
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
        modeHandler.OnTileSelected(tile);
        SoundManager.Instance.PlaySFX(clickSoundClip, 1f);
    }
    public void SortTileAndActivateShadow(List<Tile> newTiles)
    {
        SortTiles();
        ActivateShadows();
        currentTiles.AddRange(newTiles);

        
    }
    // 🔹 Logic chọn tile mặc định
    public void DefaultSelectLogic(Tile tile)
    {
        if (selectingTiles.Count >= maxSeletableTile)
        {
            GameManager.instance.ChangeState(GameState.Lose);
            return;
        }

        selectingTiles.Add(tile);
        tile.gameObject.SetActive(false);

        // Ưu tiên sắp xếp tile cùng loại
        for (int i = selectingTiles.Count - 2; i >= 0; i--)
        {
            if (selectingTiles[i].tileData == tile.tileData)
            {
                for (int j = selectingTiles.Count - 1; j > i + 1; j--)
                    (selectingTiles[j], selectingTiles[j - 1]) = (selectingTiles[j - 1], selectingTiles[j]);
                break;
            }
        }

        EventManager.OnTileSelected?.Invoke(tile);
        CheckMatch3Condition(tile.tileData);
    }

    // 🔹 Kiểm tra match 3
    public void CheckMatch3Condition(TileDataSO tileDataSO)
    {
        if (selectingTiles.Count < 3) return;

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

                modeHandler.OnTilesMatched(tileDataSO);
                break;
            }
        }

        modeHandler.OnWinCheck(currentTiles, selectingTiles);
    }

    // 🔹 Coroutine thắng level
    public IEnumerator Win()
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

            foreach (var renderer in tile.transform.Find("Container").GetComponentsInChildren<SpriteRenderer>())
            {
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

            tile.isBlocked = showShadow;

            var shadow = tile.transform.Find("Container/Shadow")?.GetComponent<SpriteRenderer>();
            if (shadow != null)
                shadow.enabled = showShadow;
        }
    }
}
