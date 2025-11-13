using UnityEngine;
using UnityEngine.UI;

public class CreateLevelBottomMenuUI : MonoBehaviour
{
    public enum TabType
    {
        Tiles,
        Levels,
        Shapes
    }
    public ScrollRect tilesScroll, levelsScroll, shapesScroll;
    public Button tilesButton, levelsButton, shapesButton;
    private TabType currentTab;
    private void Start()
    {   
        currentTab = TabType.Tiles;
        tilesScroll.gameObject.SetActive(true);
        levelsScroll.gameObject.SetActive(false);
        shapesScroll.gameObject.SetActive(false);
        tilesButton.onClick.AddListener(() =>
        {

            ShowTab(TabType.Tiles);
        }); 
        levelsButton.onClick.AddListener(() =>
        {

            ShowTab(TabType.Levels);
        });
        shapesButton.onClick.AddListener(() =>
        {

            ShowTab(TabType.Shapes);
        });
    }
    void ShowTab(TabType tab)
    {   
        if(currentTab == tab) return;  
        currentTab = tab;
        tilesScroll.gameObject.SetActive(tab == TabType.Tiles);
        levelsScroll.gameObject.SetActive(tab == TabType.Levels);
        shapesScroll.gameObject.SetActive(tab == TabType.Shapes);
    }
}
