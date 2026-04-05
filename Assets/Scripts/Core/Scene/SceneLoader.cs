// Scripts/Common/SceneLoader.cs
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static GameMode PendingMode;
    public static SceneEnum PendingScene;  // ← thêm: Loading biết phải load đâu

    public static void LoadGame(GameMode mode)
    {
        PendingMode = mode;
        PendingScene = SceneEnum.GameScene;
        SceneManager.LoadScene(SceneEnum.Loading.ToString());
    }

    public static void LoadHome()
    {
        PendingScene = SceneEnum.Home;  // ← chỉ đúng scene
        SceneManager.LoadScene(SceneEnum.Loading.ToString());
    }
}