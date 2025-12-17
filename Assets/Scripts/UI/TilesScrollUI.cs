using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesScrollUI : MonoBehaviour
{   

    public GameObject createTilePrefab;
    public List<TileDataSO> tileList;
    

    private void Start()
    {
        Initial();
    }


    private void Initial()
    {
        foreach (TileDataSO data in tileList)
        {
            GameObject tile = Instantiate(createTilePrefab, transform.Find("Viewport/Content"));
            tile.transform.Find("Food").GetComponent<Image>().sprite = data.sprite;
            tile.GetComponent<DraggableTile>().tileData = data;

        }
    }

}
