using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopAbilityUI : MonoBehaviour
{
    public TextMeshProUGUI abilityName;
    public TextMeshProUGUI abilityDescription;
    public Image abilityIcon;
    public TextMeshProUGUI abilityPrice;
    public Image currencyIcon;
    public Button purchaseButton;
    private ShopManager _shopManager;
    private ShopItemSO abilityItem;

    private void Start()
    {
        _shopManager = FindFirstObjectByType<ShopManager>(); 
    }


    private void OnEnable()
    {
        purchaseButton.onClick.AddListener(() =>
        {
            OnPurchase();
        });
    }
    private void OnDisable()
    {
     purchaseButton.onClick.RemoveAllListeners();   
    }
    public void SetupAbility(ShopItemSO shopItem)
    {   
        abilityItem = shopItem;
        abilityName.text = shopItem.name;
        abilityDescription.text = shopItem.description;
        abilityIcon.sprite = shopItem.icon;
        abilityPrice.text = shopItem.buyWith.price.ToString();
        currencyIcon.sprite = shopItem.buyWith.currencyIcon;
    }
    private void OnPurchase()
    {
        if (_shopManager != null && abilityItem != null)
        {
            _shopManager.Purchase(abilityItem);
        }
    }
}
