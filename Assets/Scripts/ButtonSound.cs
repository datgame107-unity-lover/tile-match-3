using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    Button button;

    private void Start()
    {
        button=GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX(defaultClickSound,1f);
        });
    }

    private static AudioClip defaultClickSound;
    public static void SetDefaultClickSound(AudioClip clip)
    {
        defaultClickSound = clip;
    }
}
