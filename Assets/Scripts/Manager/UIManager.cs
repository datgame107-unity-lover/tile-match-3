using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Prefabs")]
    public QuestCompleteUI questCompleteUIPrefab;

    private Transform canvas;
    private QuestCompleteUI currentPopup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.OnQuestCompleted += HandleQuestCompleted;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        EventManager.OnQuestCompleted -= HandleQuestCompleted;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCanvasInScene();

        // Khi đổi scene → popup cũ coi như invalid
        if (currentPopup != null)
        {
            Destroy(currentPopup.gameObject);
            currentPopup = null;
        }
    }

    private void FindCanvasInScene()
    {
        Canvas found = FindFirstObjectByType<Canvas>();

        if (found == null)
        {
            Debug.LogWarning("[UIManager] No Canvas found in scene.");
            canvas = null;
            return;
        }

        canvas = found.transform;
    }

    private void HandleQuestCompleted(QuestDataSO quest)
    {
        if (canvas == null)
        {
            Debug.LogWarning("[UIManager] Canvas not set. Quest popup skipped.");
            return;
        }

        // ===== KILL POPUP CŨ NGAY =====
        if (currentPopup != null)
        {
            Destroy(currentPopup.gameObject);
            currentPopup = null;
        }

        currentPopup =
            Instantiate(questCompleteUIPrefab, canvas);

        currentPopup.Setup(quest);
    }
}
