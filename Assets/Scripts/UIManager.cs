using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject selectedTilePrefab;
    [SerializeField]
    private Transform selectedTilePanel;
    private void OnEnable()
    {
        EventManager.OnTileClicked += HandleTileClicked;
        EventManager.OnTileRemoved += HandleTilesRemoved;
    }

    private void OnDisable()
    {
        EventManager.OnTileClicked -= HandleTileClicked;
        EventManager.OnTileRemoved -= HandleTilesRemoved;
    }
    private void HandleTileClicked(Tile tile)
    {
        Tile selectedTile = Instantiate(selectedTilePrefab, selectedTilePanel).GetComponent<Tile>();
        selectedTile.clicked = true;
        selectedTile.transform.GetChild(1).GetComponent<Image>().sprite = tile.tileData.sprite;
        selectedTile.tileData = tile.tileData;
        List<Tile> selectedTiles = selectedTilePanel.GetComponentsInChildren<Tile>().ToList();

        for (int i = selectedTiles.Count - 2; i >= 0; i--)
        {
            if (selectedTile.tileData == selectedTiles[i].tileData &&
                selectedTiles[i + 1].tileData != selectedTile.tileData)
            {
                SwapSibling(selectedTile.transform, selectedTiles[i + 1].transform);
            }
        }

       

        Tile[] tiles = selectedTilePanel.GetComponentsInChildren<Tile>();
        print(tiles.Length);
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
            if(tile.tileData == tileData)
            Destroy(tile.gameObject);
        }
    }
    
}
