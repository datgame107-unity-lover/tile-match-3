using UnityEngine;

public enum CurrencyType
{
    Flower,
    Diamond,
    Heart,

    Hint,
    Shuffle,
    PowerUp,
    Undo
}
[System.Serializable]
public class CurrencyData
{
    public CurrencyType currencyType;
    public Sprite currencyIcon;
    public int quantity;
}