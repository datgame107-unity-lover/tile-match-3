using UnityEngine;

public class ModeSelectionController : MonoBehaviour
{
    public static ModeSelectionController Instance;

    public GameMode SelectedMode { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SelectMode(GameMode mode)
    {
        SelectedMode = mode;
        Debug.Log("Selected Mode: " + mode);
    }
}