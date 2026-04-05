// Scripts/LevelCreate/LevelCreateManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCreateManager : MonoBehaviour
{
    public BoardController board;
    public LevelPanelUI levelPanel;

    public void OnExportPressed() => levelPanel.OnExport();
    public void OnHomePressed() => SceneManager.LoadScene(SceneEnum.Home.ToString());
    public void OnToggleGridMode()
    {
        board.gridMode = board.gridMode == GridMode.Full
            ? GridMode.Half : GridMode.Full;
    }
}