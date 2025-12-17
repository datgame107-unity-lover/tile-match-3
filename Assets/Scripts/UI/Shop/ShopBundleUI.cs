using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopBundleUI : MonoBehaviour
{
    [Header("Main UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Containers")]
    [SerializeField] private Transform abilityContainer; 
    [SerializeField] private Transform currencyContainer; 

    [Header("Prefabs")]
    [SerializeField] private GameObject abilityItemPrefab;
    [SerializeField] private GameObject currencyItemPrefab;

    [SerializeField] private Button buyButton;
    private ShopItemSO bundle;
    private void OnEnable()
    {
        buyButton.onClick.AddListener(() =>
        {
            IAPManager.Instance.BuyProduct(bundle.iapProductId);
        });      
    }
    private void OnDisable()
    {
        buyButton?.onClick.RemoveAllListeners();
    }
    public void SetupBundle(ShopItemSO data)
    {   
        bundle = data;
        if (nameText != null) nameText.text = data.bundleName;
        if (priceText != null) priceText.text = data.bundlePrice.ToString();

        foreach (Transform child in abilityContainer) Destroy(child.gameObject);
        foreach (Transform child in currencyContainer) Destroy(child.gameObject);

        foreach (var ability in data.abilities)
        {
            Transform item = Instantiate(abilityItemPrefab, abilityContainer).transform;

            item.Find("Container/AbilityIcon").GetComponent<Image>().sprite = ability.abilityIcon;
            item.Find("Container/AbilityQuantity").GetComponent<TextMeshProUGUI>().text = $"{ability.quantity}";
            item.Find("Container/AbilityName").GetComponent<TextMeshProUGUI>().text = ability.abilityName.ToString();
        }

        foreach (var currency in data.currencies)
        {
            Transform item = Instantiate(currencyItemPrefab, currencyContainer).transform;

            item.Find("CurrencyIcon").GetComponent<Image>().sprite = currency.currencyIcon;
            item.Find("CurrencyQuantity").GetComponent<TextMeshProUGUI>().text = currency.quantity.ToString();
        }
    }
}