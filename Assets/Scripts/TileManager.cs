using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private int selectableCount;
    List<Tile> tileLists;
    List<Tile> selectedTiles;

    private void Start()
    {
        tileLists = GetComponentsInChildren<Tile>().ToList();
        selectedTiles = new List<Tile>(selectableCount);
    }

    private void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {
            Tile clickedTile = RayCastTile();
            if (clickedTile != null&&!clickedTile.clicked&&!clickedTile.isBlocked)
            {
                SelectTile(clickedTile);
                EventManager.OnTileClicked?.Invoke(clickedTile);

            }
        }
    }

    private Tile RayCastTile()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
        if (hits.Length == 0) return null;

        RaycastHit2D firstHit = hits[0];
        Tile clickedTile = firstHit.collider.GetComponent<Tile>();
        if (clickedTile == null) return null;

        foreach (var hit in hits)
        {
            Tile otherTile = hit.collider.GetComponent<Tile>();
            if (otherTile != null && otherTile.layer > clickedTile.layer)
            {   
                return null;
            }
        }

        return clickedTile; 
    }

    private void SelectTile(Tile tile)
    {   
        tile.clicked = true;
        selectedTiles.Add(tile);
        tile.gameObject.SetActive(false);
        RevealNextLayer(tile);
        CheckSelectedTiles(selectedTiles,tile);
    }


    private void RevealNextLayer(Tile tile)
    {

    }

    private void CheckSelectedTiles(List<Tile> selectedTiles,Tile selectedTile)
    {
        if (selectedTiles.Count < 3) return;
        List<Tile> tilesToDelete = new List<Tile>();
        foreach (Tile tile in selectedTiles)
        {   
            if(tile.tileData == selectedTile.tileData)
            {
                tilesToDelete.Add(tile);
            }
        }
        print(tilesToDelete.Count);
        if (tilesToDelete.Count > 2)
        {   
            
            DeleteTiles(tilesToDelete);
        }
        if (selectedTiles.Count >= selectableCount)

        {

        }
      
    }

    private void DeleteTiles(List<Tile> tiles)
    {   
        foreach(Tile tile in tiles)
        {
            selectedTiles.Remove(tile);

            DestroyImmediate(tile.gameObject);
        }
        StartCoroutine(InvokeRemoveNextFrame(tiles[0].tileData));

    }
    private IEnumerator InvokeRemoveNextFrame(TileDataSO tileData)
    {
        yield return null; // đợi 1 frame
        EventManager.OnTileRemoved?.Invoke(tileData);

    }

}
