// Scripts/LevelCreate/ScrollTabController.cs
using UnityEngine;
using UnityEngine.UI;

public class ScrollTabController : MonoBehaviour
{
    public Button tilesButton;
    public Button levelsButton;
    public GameObject tilesScroll;
    public GameObject levelsScroll;

    public Color activeColor = new Color(0.3f, 0.85f, 0.8f);
    public Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);

    private void Start()
    {
        tilesButton.onClick.AddListener(() => ShowTab(0));
        levelsButton.onClick.AddListener(() => ShowTab(1));
        ShowTab(0);
    }

    private void ShowTab(int index)
    {
        tilesScroll.SetActive(index == 0);
        levelsScroll.SetActive(index == 1);
        tilesButton.GetComponent<Image>().color = index == 0 ? activeColor : inactiveColor;
        levelsButton.GetComponent<Image>().color = index == 1 ? activeColor : inactiveColor;
    }
}