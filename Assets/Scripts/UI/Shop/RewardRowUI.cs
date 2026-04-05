using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardRowUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amount;

    // ===============================
    // Currency
    // ===============================
    public void BindCurrency(CurrencyData c)
    {
        icon.sprite = c.icon;
        amount.text = $"x{c.quantity}";
    }

    // ===============================
    // Ability
    // ===============================
    public void BindAbility(AbilityData a)
    {
        icon.sprite = a.icon;
        amount.text = $"x{a.quantity}";
    }
}