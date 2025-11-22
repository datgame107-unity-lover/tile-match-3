using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WarningType
{
    Delete,
    Unsave,
    EmptyTile,
    Restart
}

[Serializable]
public class WarningData
{
    public WarningType warningType;
    public string message;
    public string agreeText;
    public string refuseText;

    public Action agreeAction;
    public Action refuseAction;
}
public class WarningPanel : MonoBehaviour
{
    [Header("UI References")]
    public Button agreeButton;
    public Button refuseButton;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI agreeButtonText;
    public TextMeshProUGUI refuseButtonText;

    public void ShowWarning(WarningData data)
    {
        gameObject.SetActive(true);

        messageText.text = data.message;
        agreeButtonText.text = data.agreeText;
        refuseButtonText.text = data.refuseText;

        agreeButton.onClick.RemoveAllListeners();
        refuseButton.onClick.RemoveAllListeners();

        agreeButton.onClick.AddListener(() =>
        {
            data.agreeAction?.Invoke();
            HidePanel();
        });

        refuseButton.onClick.AddListener(() =>
        {
            data.refuseAction?.Invoke();
            HidePanel();
        });
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
