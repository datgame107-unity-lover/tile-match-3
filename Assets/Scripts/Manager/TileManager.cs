using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class TileManager : MonoBehaviour
{
    [Header("Board")]
    public GameObject tilePrefab;

    [Header("Tile Datas")]
    public List<TileDataSO> tileDatas;

    [Header("Settings")]
    public int maxSelectableTile = 9;
    public AudioClip clickSoundClip;

    [Header("Refs / Debug")]
    public List<Tile> currentTiles = new List<Tile>();
    public List<Tile> selectingTiles = new List<Tile>();

    private IGameModeHandler modeHandler;

    private Tile currentHoveredTile;

    private Stack<Tile> undoStack = new Stack<Tile>();

    private bool isUsingPowerUp = false;
    private bool isDiscarding = false;


    private void Awake()
    {
        currentTiles = new List<Tile>();
        selectingTiles = new List<Tile>();
    }

    private void Start()
    {
        Debug.Log($"[TileManager] Current Level: {PlayerPrefs.GetInt("level")}");
        GenerateNewGame(GameManager.instance.gameMode);

        RefreshCurrentTilesFromHierarchy();

        // Setup mode handler
        switch (GameManager.instance.gameMode)
        {
            case GameMode.Level:
                modeHandler = new LevelModeHandler(this);
                break;
            case GameMode.Infinite:
                modeHandler = new InfiniteModeHandler(this);
                break;
            default:
                modeHandler = new LevelModeHandler(this);
                break;
        }
    }

    private void OnEnable()
    {
        EventManager.OnContinueButtonClicked += HandleContinueLevel;
        EventManager.OnRestartLevel += RestartLevelHandler; 
        EventManager.OnPlayOn += PlayonHandler;
    }

    private void OnDisable()
    {
        EventManager.OnRestartLevel -= RestartLevelHandler;

        EventManager.OnContinueButtonClicked -= HandleContinueLevel;
    }

    private void PlayonHandler()
    {
        for (int i = selectingTiles.Count-1; selectingTiles.Count>4; i--)
        {
            selectingTiles[i].gameObject.SetActive(true);
            selectingTiles[i].isClicked = false;
            selectingTiles[i].transform.Find("Container").localScale = Vector3.zero;
            selectingTiles[i].transform.Find("Container").DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            selectingTiles.Remove(selectingTiles[i]);

        }
        GameManager.instance.ChangeState( GameState.Playing );
    }
    private void LateUpdate()
    {
        SortTiles.Sort(currentTiles);
        SortTiles.ActivateShadows(currentTiles);
    }

    private void Update()
    {
        if (GameManager.instance.currentState != GameState.Playing) return;

        Tile tileUnderMouse = GetTopTileUnderMouse();

        if (Input.GetMouseButton(0))
        {
            if (tileUnderMouse != currentHoveredTile)
            {
                if (currentHoveredTile != null)
                {
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container"), 1f);
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container"), 1f);

                }

                currentHoveredTile = tileUnderMouse;

                if (currentHoveredTile != null && !currentHoveredTile.isBlocked)
                {
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container"), 1.2f);
                    DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container"), 1f);

                }
            }

            if (tileUnderMouse == null && currentHoveredTile != null)
            {
                DOAnimationManager.ScaleBounce(currentHoveredTile.transform.Find("Container"), 1f);
                currentHoveredTile = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentHoveredTile != null && !currentHoveredTile.isBlocked&&!currentHoveredTile.isClicked)
            {
                SelectTile(currentHoveredTile);
                currentHoveredTile = null;
            }
        }
    }

    #region --- Game Setup / Mode ---
    private void RestartLevelHandler()
    {
        RestartLevel();
    }

    private void RestartLevel()
    {
        GenerateNewLevel(GameManager.instance.level);
        currentTiles.Clear();
        selectingTiles.Clear();
    }
    private void HandleContinueLevel()
    {
        GenerateNewGame(GameMode.Level);
    }

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
        for (int i = 0; i < maxLayer; i++)
        {
            //LevelManager.Instance.GenerateOneLayer(i, 21);
        }

        RefreshCurrentTilesFromHierarchy();
     
        GameManager.instance.ChangeState(GameState.Playing);
    }

    private void GenerateNewLevel(int level)
    {
        for (int i = currentTiles.Count - 1; i >= 0; i--)
        {
            Destroy(currentTiles[i].gameObject);
        }
        SortTiles.Sort(currentTiles);
        SortTiles.ActivateShadows(currentTiles);
        GameManager.instance.ChangeState(GameState.Playing);
        LevelDataManager.LoadFromSO(PlayerPrefs.GetInt("level"), tilePrefab, this.transform);
        RefreshCurrentTilesFromHierarchy();
    }

    private void RefreshCurrentTilesFromHierarchy()
    {
        currentTiles = GetComponentsInChildren<Tile>().ToList();
        
        selectingTiles = new List<Tile>();
        undoStack.Clear();
    }



    #endregion

    #region --- Input / Select / Modes ---

    private Tile GetTopTileUnderMouse()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        int highestLayer = int.MinValue;
        Tile topTile = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            Tile t = hit.collider.GetComponent<Tile>();
            if (t == null) continue;

            if (t.layer >= highestLayer)
            {
                highestLayer = t.layer;
                topTile = t;
            }
        }

        return topTile;
    }

    public void SelectTile(Tile tile)
    {
        if (tile == null && tile.isClicked) return;
        tile.isClicked = true;
        if (isUsingPowerUp)
        {
            TryUsePowerUp(tile);
            modeHandler?.OnWinCheck(currentTiles, selectingTiles);
            return;
        }

        if (isDiscarding)
        {
            TryDiscard(tile);
            modeHandler?.OnWinCheck(currentTiles, selectingTiles);
            return;
        }

        modeHandler?.OnTileSelected(tile);
        SoundManager.Instance?.PlaySFX(clickSoundClip, 1f);
    }

    #endregion

    #region --- Default selection & match3 ---

    public void DefaultSelectLogic(Tile tile)
    {
        if (tile == null) return;

        if (selectingTiles.Count >= maxSelectableTile)
        {
            tile.isClicked = false;
            GameManager.instance.ChangeState(GameState.Lose);
           
            return;
        }

        undoStack.Push(tile);

        selectingTiles.Add(tile);
        tile.gameObject.SetActive(false);

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

    private void CheckMatch3Condition(TileDataSO tileDataSO)
    {
        if (selectingTiles.Count < 3) return;

        for (int i = 0; i <= selectingTiles.Count - 3; i++)
        {
            var t1 = selectingTiles[i];
            var t2 = selectingTiles[i + 1];
            var t3 = selectingTiles[i + 2];

            if (t1 == null || t2 == null || t3 == null) continue;

            if (t1.tileData == t2.tileData && t1.tileData == t3.tileData)
            {
                List<Tile> tilesToRemove = new List<Tile> { t1, t2, t3 };

                foreach (var t in tilesToRemove)
                {
                    currentTiles.Remove(t);
                    selectingTiles.Remove(t);


                    if (t != null && t.gameObject != null)
                        Destroy(t.gameObject);
                }

                modeHandler?.OnTilesMatched(tileDataSO,t3   );

                break;
            }
        }

        modeHandler?.OnWinCheck(currentTiles, selectingTiles);
    }

    public IEnumerator Win()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.ChangeState(GameState.Win);
    }

    #endregion

    #region --- Sort & Shadow ---

   

    #endregion

    #region --- PowerUp / Discard ---

    public void PowerUp()
    {
        isUsingPowerUp = true;
        isDiscarding = false;
        Debug.Log("[TileManager] PowerUp activated - click a tile to clear its triplet");
    }

    private void TryUsePowerUp(Tile tile)
    {
        if (!isUsingPowerUp) return;
        isUsingPowerUp = false;

        if (tile == null) return;

        List<Tile> same = selectingTiles
            .Where(t => t != null && t.tileData == tile.tileData)
            .ToList();
        print(same.Count);
        if (same.Count < 2)
        {
            same.AddRange(
                currentTiles.Where(t => t != null
                    && t.tileData == tile.tileData
                    && t != tile)
            );
        }

        if (same.Count < 2)
        {
            Debug.Log("[TileManager] PowerUp: not enough tiles to clear triple");
            return;
        }

        List<Tile> toRemove = same.Take(2).ToList();
        toRemove.Add(tile);

        // XÓA: delete selecting trước -> currentTiles sau
        foreach (var t in toRemove)
        {
            if (t == null) continue;

            selectingTiles.Remove(t);   // always remove selecting first
            currentTiles.Remove(t);     // then safe remove in currentTiles
            Destroy(t.gameObject);
        }
        EventManager.OnTilesRemoved?.Invoke(tile.tileData);

        modeHandler?.OnTilesMatched(tile.tileData,tile);
        SoundManager.Instance?.PlaySFX(clickSoundClip, 1f);

        Debug.Log("[TileManager] PowerUp used - removed 3 tiles of type: " + tile.tileData.name);
    }


    public void Discard()
    {
        isDiscarding = true;
        isUsingPowerUp = false;
        Debug.Log("[TileManager] Discard activated - click a tile to remove it");
    }

    private void TryDiscard(Tile tile)
    {
        if (!isDiscarding) return;
        isDiscarding = false;

        if (tile == null) return;

        currentTiles.Remove(tile);

        if (tile.gameObject != null)
            Destroy(tile.gameObject);

        SoundManager.Instance?.PlaySFX(clickSoundClip, 1f);
        Debug.Log("[TileManager] Discard used - removed 1 tile");
    }

    #endregion

    #region --- Shuffle / Hint / Undo (public UI bindings) ---


    public void Shuffle()
    {
        if (currentTiles == null || currentTiles.Count <= 1) return;

        List<Vector3> positions = currentTiles.Select(t => t.transform.position).ToList();

        for (int i = positions.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (positions[i], positions[rand]) = (positions[rand], positions[i]);
        }

        // Gán lại
        for (int i = 0; i < currentTiles.Count; i++)
        {
            if (currentTiles[i] == null) continue;
            currentTiles[i].transform.position = positions[i];
        }

        Debug.Log("[TileManager] Shuffle completed");
    }
    public void RefreshBoard()
    {
        List<Tile> allTiles = currentTiles
            .Concat(selectingTiles)
            .Where(t => t != null)
            .ToList();

        var tileCounts = allTiles
            .GroupBy(t => t.tileData)
            .ToDictionary(g => g.Key, g => g.Count());

        bool hasMatchable = tileCounts.Values.Any(count => count >= 3);

        if (!hasMatchable)
        {
            Debug.Log("[TileManager] RefreshBoard: No matchable set found, assigning 3 tiles to a random type.");

            TileDataSO randomType = allTiles[Random.Range(0, allTiles.Count)].tileData;

            List<Tile> tilesToChange = allTiles.OrderBy(t => Random.value).Take(3).ToList();

            foreach (var tile in tilesToChange)
            {
                tile.tileData = randomType;
                tile.transform.Find("Container/Food").GetComponent<SpriteRenderer>().sprite = randomType.sprite;
            }

            currentTiles = currentTiles.OrderBy(t => Random.value).ToList();
        }

    }
    public void Hint()
    {
        if (currentTiles == null || currentTiles.Count == 0) return;

        var group = currentTiles
            .Where(t => t != null)
            .GroupBy(t => t.tileData)
            .Where(g => g.Count() >= 3)
            .FirstOrDefault();

        if (group == null)
        {
            Debug.Log("[TileManager] Hint: no available match");
            return;
        }

        List<Tile> hintTiles = group.Take(3).ToList();

        foreach (var tile in hintTiles)
        {
            if (tile == null) continue;
            DOAnimationManager.ScaleBounce(tile.transform.Find("Container"), 1.3f, 0.1f);
        }

        Debug.Log("[TileManager] Hint shown");
    }

    public void Undo()
    {
        while (undoStack.Count > 0)
        {
            Tile last = undoStack.Pop();
            if (last == null) continue; // tile destroyed
            if (last.gameObject == null) continue; // destroyed

            if (last.gameObject.activeSelf)
            {
                continue;
            }

            last.gameObject.SetActive(true);

            selectingTiles.Remove(last);

            if (!currentTiles.Contains(last))
                currentTiles.Add(last);
            EventManager.OnTileRemoved?.Invoke(last.tileData);
  

            Debug.Log("[TileManager] Undo: restored last selected tile");
            return;
        }

        Debug.Log("[TileManager] Undo: nothing to restore");
    }

    #endregion

    #region --- Helpers / UI bindings ---

    public void OnShuffleButton() => Shuffle();
    public void OnUndoButton() => Undo();
    public void OnHintButton() => Hint();
    public void OnPowerUpButton() => PowerUp();


    public void SortTileAndActivateShadow(List<Tile> newTiles)
    {
        if (newTiles == null || newTiles.Count == 0) return;

        foreach (var t in newTiles)
        {
            if (t == null) continue;
            if (!currentTiles.Contains(t)) currentTiles.Add(t);
        }

      
    }

    #endregion
}
