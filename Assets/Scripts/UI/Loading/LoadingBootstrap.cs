// Scripts/Common/LoadingBootstrap.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBootstrap : MonoBehaviour
{
    [SerializeField] private Image progressFill;
    [SerializeField] private float minLoadTime = 1f; // thời gian tối thiểu hiển thị
    [SerializeField] private float smoothSpeed = 3f;   // tốc độ lerp progress bar

    private void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        string targetScene = SceneLoader.PendingScene.ToString();

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        float displayFill = 0f; // giá trị hiển thị — lerp mượt

        while (true)
        {
            elapsed += Time.deltaTime;

            // progress thực từ Unity (0 → 0.9)
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            // progress hiển thị = min của real và thời gian đã trôi qua
            float timeProgress = Mathf.Clamp01(elapsed / minLoadTime);
            float targetFill = Mathf.Min(realProgress, timeProgress);

            // lerp mượt
            displayFill = Mathf.Lerp(displayFill, targetFill, Time.deltaTime * smoothSpeed);

            if (progressFill) progressFill.fillAmount = displayFill;

            // chỉ activate khi cả load thật lẫn thời gian tối thiểu đều xong
            bool loadReady = op.progress >= 0.9f;
            bool timeReady = elapsed >= minLoadTime;

            if (loadReady && timeReady && displayFill >= 0.98f)
            {
                progressFill.fillAmount = 1f;
                yield return new WaitForSeconds(0.1f);
                op.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }
}