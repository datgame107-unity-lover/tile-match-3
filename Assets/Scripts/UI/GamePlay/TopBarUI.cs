using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarUI : MonoBehaviour
{
    [SerializeField]
    private Button pauseButton;
    private TextMeshProUGUI flowerText;
    private TextMeshProUGUI diamondText;

    [SerializeField]
    private SettingsPanel settingsPanel;


    private void OnEnable()
    {
        pauseButton.onClick.AddListener(Show);
    }

    private void Show()
    {
            settingsPanel.Show();
    }

}
