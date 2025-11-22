using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCreateUI : MonoBehaviour
{
    public Button saveButton;
    public Button homeButton;
    public Button tilesButton, levelsButton, shapesButton;
    public ScrollRect tilesScroll, levelsScroll, shapesScroll;
    public enum TabType
    {
        Tiles,
        Levels,
        Shapes
    }
    
    private TabType currentTab;
  
    void ShowTab(TabType tab)
    {
        if (currentTab == tab) return;
        currentTab = tab;
        tilesScroll.gameObject.SetActive(tab == TabType.Tiles);
        levelsScroll.gameObject.SetActive(tab == TabType.Levels);
        shapesScroll.gameObject.SetActive(tab == TabType.Shapes);
    }
    private void Start()
    {
        saveButton.onClick.AddListener(() =>
        {
          
            EventManager.OnSavingNewLevel?.Invoke();
        });
        homeButton.onClick.AddListener(() =>
        {
            SceneLoader.TargetScene = SceneEnum.Home; // đặt scene muốn load
            SceneManager.LoadScene(SceneEnum.Loading.ToString(), LoadSceneMode.Single);

        });
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

   
}
