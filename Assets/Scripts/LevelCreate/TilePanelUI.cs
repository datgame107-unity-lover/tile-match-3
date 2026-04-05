// Scripts/LevelCreate/TilePanelUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TilePanelUI : MonoBehaviour
{
    public BoardController board;
    public Transform listContainer;
    public GameObject tileItemPrefab;
    public TileDataSO[] allTileData;

    private void Start()
    {
        foreach (var data in allTileData)
        {
            var item = Instantiate(tileItemPrefab, listContainer);

            // lấy SpriteRenderer của Food để set sprite
            var food = item.transform.Find("Food");
                var sr = food.GetComponent<Image>();
                if (sr != null) sr.sprite = data.sprite;

            // gán drag handler
            var drag = item.AddComponent<DragTileHandler>();
            drag.tileData = data;
            drag.board = board;
        }
    }
}