using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public enum SceneEnum
{
    Home,
    Loading,
    GameScene
}

public class SceneLoader : MonoBehaviour
{
    public static SceneEnum TargetScene = SceneEnum.GameScene;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image fillImage; // image của slider để làm fade

    private void Start()
    {
        progressBar.value = 0;
        StartCoroutine(LoadTargetSceneAsync());

        // Chớp tắt fill image
        fillImage.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private IEnumerator LoadTargetSceneAsync()
    {
        string sceneName = TargetScene.ToString();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f; // giá trị đang hiển thị

        while (!operation.isDone)
        {
            // target progress (0 -> 1)
            float targetValue = Mathf.Clamp01(operation.progress / 0.2f);

            // Lerp chậm cho mượt
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetValue, Time.deltaTime * 0.5f);
            progressBar.value = displayedProgress;

            if (operation.progress >= 0.9f && displayedProgress >= 0.99f)
            {
                yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

}
