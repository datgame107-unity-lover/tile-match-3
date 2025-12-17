using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemData : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText; // Optional (assign only for Ability)
    [SerializeField] private TextMeshProUGUI priceText;

    public void Setup(ShopItemSO data)
    {
        if (nameText) nameText.text = data.name;
        if (priceText) priceText.text = data.buyWith.price.ToString();
        if (iconImage) iconImage.sprite = data.icon;

        // Only abilities usually have descriptions
        if (descriptionText != null)
            descriptionText.text = data.description;
    }

}
