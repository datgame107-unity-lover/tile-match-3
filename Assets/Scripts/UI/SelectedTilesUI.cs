using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedTilesUI : MonoBehaviour
{

    [SerializeField]
    private GameObject selectedTilePrefab;
    [SerializeField]
    private RectTransform selectedTilePanel;
    private void OnEnable()
    {
        EventManager.OnTileSelected += HandleTileSelected;
        EventManager.OnTileRemoved += HandleTilesRemoved;
    }

    private void OnDisable()
    {
        EventManager.OnTileSelected -= HandleTileSelected;
        EventManager.OnTileRemoved -= HandleTilesRemoved;
    }


    public void HandleTileSelected(Tile tile)
    {
        Canvas rootCanvas = selectedTilePanel.GetComponentInParent<Canvas>();

        Tile selectedTile = Instantiate(selectedTilePrefab, selectedTilePanel).GetComponent<Tile>();
        RectTransform rt = selectedTile.GetComponent<RectTransform>();

        // Chuyển tọa độ từ World sang Local của panel
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            selectedTilePanel.GetComponent<RectTransform>(),
            Camera.main.WorldToScreenPoint(tile.transform.position),
            rootCanvas.worldCamera,
            out localPos
        );
        rt.anchoredPosition = localPos;

        selectedTile.isClicked = true;
        selectedTile.transform.Find("Container/Food").GetComponent<Image>().sprite = tile.tileData.sprite;
        selectedTile.tileData = tile.tileData;

        Tile[] selectedTiles = selectedTilePanel.GetComponentsInChildren<Tile>();
        for (int i = selectedTiles.Length - 2; i >= 0; i--)
        {
            if (selectedTiles[i].tileData == tile.tileData && selectedTiles[i + 1] != tile.tileData)
            {
                SwapSibling(selectedTile.transform, selectedTiles[i + 1].transform);
                break;
            }
        }
        // Shake rotation Z
        Vector3 localScale = selectedTile.transform.Find("Container").localScale;
        selectedTile.transform.Find("Container").DOScale(1.4f, 0.5f)
      .OnComplete(() => selectedTile.transform.Find("Container").localScale = localScale);


    }
  

    private void SwapSibling(Transform sibling1, Transform sibling2)
    {
        int index1 = sibling1.GetSiblingIndex();
        int index2 = sibling2.GetSiblingIndex();

        sibling1.SetSiblingIndex(index2);
        sibling2.SetSiblingIndex(index1);
    }


    private void HandleTilesRemoved(TileDataSO tileData)
    {
        List<Tile> selectedTiles = new List<Tile>();
        selectedTiles = selectedTilePanel.GetComponentsInChildren<Tile>().ToList();
        foreach (Tile tile in selectedTiles)
        {   
            if (tile.tileData == tileData)
            {
                tile.transform.Find("Container").DOScale(1.2f, .3f).SetLoops(1, LoopType.Restart)
                     .OnComplete(() =>
                     {
                         Destroy(tile.gameObject);
                     });
            }
        }
    }

}
