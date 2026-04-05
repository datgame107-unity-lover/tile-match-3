// ShopItemView.cs

using UnityEngine;

public abstract class ShopItemView : MonoBehaviour
{
    public abstract void Bind(
        ShopItemSO item,
        ShopService service);
}