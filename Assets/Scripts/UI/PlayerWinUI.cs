using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerWinUI : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerWinUIPanel;
    
    private void OnEnable()
    {
        EventManager.OnPlayerWon += HandlePlayerWin;
    }
    private void OnDisable()
    {
        EventManager.OnPlayerWon -= HandlePlayerWin;
    }

    public void ContinueButtonClick()
    {
        HideUI();
        EventManager.OnContinueButtonClicked?.Invoke();
    }
    public void HomeButtonClick()
    {
        HideUI();

        SceneLoader.TargetScene = SceneEnum.Home; // đặt scene muốn load
        SceneManager.LoadScene(SceneEnum.Loading.ToString(),LoadSceneMode.Single);
    }
    private void HandlePlayerWin()
    {
        PlayerWinUIPanel.SetActive(true);
    }
    private void HideUI()
    {
        PlayerWinUIPanel.SetActive(false);
    }
}
