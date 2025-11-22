using System.Linq;
using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public static LevelEditorManager Instance;
    public bool isChanged = false;
    public int currentLevel;
    public Transform grid;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        currentLevel = LevelDataManager.GetTotalLevel();
    }
    private void OnEnable()
    {
        EventManager.OnSavingNewLevel += SaveNewLevelHandler;
    }
    private void OnDisable()
    {
        EventManager.OnSavingNewLevel -= SaveNewLevelHandler;

    }
    public void SaveNewLevelHandler()
    {

        if (LevelDataManager.SaveToSO(grid, currentLevel+1))
        {
            print("ok");
            EventManager.OnSavedNewLevel?.Invoke();
        }
    }
    private void LateUpdate()
    {
        Sort();
    }
    public void Sort()
    {
        SortTiles.Sort(grid.GetComponentsInChildren<Tile>().ToList());
        SortTiles.ActivateShadows(grid.GetComponentsInChildren<Tile>().ToList());
    }
}
