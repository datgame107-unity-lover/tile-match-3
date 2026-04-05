// Scripts/LevelCreate/ConfirmDialogUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmDialogUI : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    private System.Action _onConfirm;
    private System.Action _onCancel;

    private void Awake()
    {
        gameObject.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    public void Show(string message, System.Action onConfirm, System.Action onCancel)
    {
        messageText.text = message;
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        gameObject.SetActive(true);
    }

    private void OnConfirm() { gameObject.SetActive(false); _onConfirm?.Invoke(); }
    private void OnCancel() { gameObject.SetActive(false); _onCancel?.Invoke(); }
}