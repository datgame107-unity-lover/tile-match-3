using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerLoseUI : MonoBehaviour
{
    public Image lostImage;
    public Button playOnButton;
    public Button giveUpButton;
    public WarningPanel warningPanel;


    
    private void Start()
    {

        

        playOnButton.onClick.AddListener(() =>
        {
            EventManager.OnPlayOn?.Invoke();
            this.gameObject.SetActive(false);
        });
        giveUpButton.onClick.AddListener(() =>
        {
            warningPanel.gameObject.SetActive(true);
            WarningData data = new WarningData()
            {
                warningType = WarningType.Delete,
                message = "Do you Want to give up?",
                agreeText = "Restart",
                refuseText = "Home",

                agreeAction = () =>
                {
                    EventManager.OnRestartLevel?.Invoke();

                    GameManager.instance.ChangeState(GameState.Playing); 
                    this.gameObject.SetActive(false);
                },
                refuseAction = () =>
                {
                    SceneLoader.TargetScene = SceneEnum.Home; // đặt scene muốn load
                    SceneManager.LoadScene(SceneEnum.Loading.ToString(), LoadSceneMode.Single);

                }
            };



            warningPanel.ShowWarning(data);
        });

    }

    private void OnEnable()
    {
        RectTransform rt = lostImage.rectTransform;

        Vector2 endPos = rt.anchoredPosition;

        rt.anchoredPosition = new Vector2(endPos.x, endPos.y + 800f);

        rt.DOAnchorPos(endPos, 0.6f)
            .SetEase(Ease.OutBounce);
    }

}
