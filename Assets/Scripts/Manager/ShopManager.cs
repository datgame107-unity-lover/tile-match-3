using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private List<ShopItemSO> shopItems;
    public List<ShopItemSO> GetItemsByType (ShopItemType type)
    {
         return shopItems.FindAll(i=>i.itemType==type);
    }
    private void OnEnable()
    {
        
    }
}
