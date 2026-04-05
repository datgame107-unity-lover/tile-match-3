// Scripts/LevelCreate/LevelItemUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelItemUI : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI levelLabel;
    public TextMeshProUGUI levelIndexText;
    public Button itemButton;
    public Button deleteButton;
    public Image background;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.6f, 0.9f, 0.6f);

    private int _index;
    private LevelPanelUI _panel;

    public void Init(int index, bool isEditing, bool isDirty, LevelPanelUI panel)
    {
        _index = index;
        _panel = panel;

        if (levelLabel != null) levelLabel.text = "LEVEL";
        if (levelIndexText != null) levelIndexText.text = isDirty ? $"{index} *" : $"{index}";
        if (background != null) background.color = isEditing ? selectedColor : normalColor;

        if (itemButton != null)
            itemButton.onClick.AddListener(() => _panel.OnClickItem(_index));
        if (deleteButton != null)
            deleteButton.onClick.AddListener(() => _panel.OnDelete(_index));
    }
}