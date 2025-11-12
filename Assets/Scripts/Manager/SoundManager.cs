using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Settings")]
    public bool musicOn = true;
    public bool sfxOn = true;
    public bool vibrationOn = true;

    [Header("Audio Clips")]
    public List<AudioClip> backgroundMusics;
    public AudioClip clickSoundClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource.mute = !musicOn;
        sfxSource.mute = !sfxOn;

        ButtonSound.SetDefaultClickSound(clickSoundClip);

        if (musicOn && backgroundMusics != null)
        {
            PlayMusic(backgroundMusics[Random.Range(0,backgroundMusics.Count-1)]);
        }
    }


    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;

        if (musicOn)
            musicSource.Play();
        else
            musicSource.Stop(); // nếu tắt, dừng hẳn
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusic(bool on)
    {
        musicOn = on;

        if (musicOn)
        {
            musicSource.mute = false;

            if (!musicSource.isPlaying && backgroundMusics != null)
                PlayMusic(backgroundMusics[Random.Range(0, backgroundMusics.Count - 1)]);

        }
        else
        {
            musicSource.mute = true;
            musicSource.Pause(); // ngừng nhưng không reset clip
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!sfxOn || clip == null) return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void SetSFX(bool on)
    {
        sfxOn = on;
        sfxSource.mute = !on;
    }


    public void Vibrate(float duration = 0.1f)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (vibrationOn)
        {
            Handheld.Vibrate();
        }
#endif
    }

    public void SetVibration(bool on)
    {
        vibrationOn = on;
    }
}
