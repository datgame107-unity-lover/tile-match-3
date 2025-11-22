using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public enum GameState
{
    Playing,
    Pause,
    Lose,
    Win,
    Creating,
}
public enum GameMode
{
    Level,
    Infinite
}
public class GameManager : MonoBehaviour
{   
    public static GameManager instance;
    public GameState currentState { get; private set; }
    public GameMode gameMode { get; private set; }
    public int level;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
        else
        {
            Destroy(this.gameObject);
            return;
        }
        //InnitialData();
        level = PlayerPrefs.GetInt("level");
        DontDestroyOnLoad(instance);
       
    }
    private void OnEnable()
    {
        EventManager.OnContinueButtonClicked += HandleContinue;
    }

    private void OnDisable()
    {
        EventManager.OnContinueButtonClicked -= HandleContinue;
    }
    
    private void InnitialData()
    {
        PlayerPrefs.SetString("player_name","hihihi");
        PlayerPrefs.SetInt("music_on", 1);
        PlayerPrefs.SetInt("sfx_on", 1);
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetInt("no_ads", 0);
    }
    private void HandleContinue()
    {
        // chuyển sang trạng thái tạo level mới
        ChangeState(GameState.Creating);
    }
    public void ChangeState(GameState state)
    {
        if (state == currentState) return;

        currentState = state;

        EventManager.OnStateChanged?.Invoke(state);
        switch (state)
        {
            case GameState.Creating:
                HandleCreating();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.Pause:
                HandlePause();
                break;
            case GameState.Win:
                HandleWin();
                break;
            case GameState.Lose:
                HandleLose();
                break;
        }

    }
    private void HandleCreating()
    {
    }

    private void HandlePlaying()
    {
    }

    private void HandlePause()
    {
    }

    private void HandleWin()
    {
        PlayerPrefs.SetInt("level", level+1);
        PlayerPrefs.Save();
        SoundManager.Instance.PlayWinClip(1);
        EventManager.OnPlayerWon?.Invoke();
    }

    private void HandleLose()
    {

        EventManager.OnPlayerLost?.Invoke();
        SoundManager.Instance.PlayWinClip(1);

    }
    public void ChangeMode(GameMode mode)
    {
        this.gameMode = mode;
    }
    public void SaveLevel(int newLevel)
    {
        level = newLevel;
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.Save();
        Debug.Log($"💾 Đã lưu level: {level}");
    }
}
