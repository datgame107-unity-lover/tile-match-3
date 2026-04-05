using UnityEngine;
using UnityEngine.UI;

public class ModeItemUI : MonoBehaviour
{   
    private GameMode gameMode;
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        ModeSelectionController.Instance.SelectMode(gameMode);
    }
}
