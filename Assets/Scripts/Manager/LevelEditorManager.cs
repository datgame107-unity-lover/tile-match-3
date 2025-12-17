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
        EventManager.OnChoseLevel += ChooseLevelHandler;
    }
    private void OnDisable()
    {
        EventManager.OnSavingNewLevel -= SaveNewLevelHandler;
        EventManager.OnChoseLevel -= ChooseLevelHandler;

    }
    public void ChooseLevelHandler(int level)
    {
        currentLevel = level;
    }
    public void SaveNewLevelHandler()
    {
        if (currentLevel == 0) currentLevel = 1;
        if (LevelDataManager.SaveToSO(grid, currentLevel))
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
