using UnityEngine;
using System.Collections.Generic;

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
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip tileRemoveClip;
    public AudioClip claimQuestClip;
    public AudioClip buyItemClip;
    public AudioClip successClip;
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

        // load trạng thái ngay từ Awake
        musicOn = PlayerPrefs.GetInt("music_on", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("sfs_on", 1) == 1;

        musicSource.mute = !musicOn;
        sfxSource.mute = !sfxOn;

        ButtonSound.SetDefaultClickSound(clickSoundClip);

        if (musicOn && backgroundMusics != null && backgroundMusics.Count > 0)
            PlayMusic(backgroundMusics[Random.Range(0, backgroundMusics.Count)]);
    }

    private void OnEnable()
    {
        EventManager.OnPlayerLost += HandlePlayerLost;
        EventManager.OnPlayerWon += HandlePlayerWon;
        EventManager.OnTilesRemoved += HandleTilesRemoved;
        EventManager.OnTileSelected += HandleTileSelected;
        EventManager.OnQuestClaimed += HandleQuestClaimed;
        EventManager.OnBoughtItem += HandleBoughtItem;
        EventManager.OnTransactionComplete += HandleTransactionComplete;
        EventManager.OnQuestCompleted += HandleQuestClaimed;
    }
    private void OnDisable()
    {
        EventManager.OnPlayerLost -= HandlePlayerLost;
        EventManager.OnPlayerWon -= HandlePlayerWon;
        EventManager.OnTilesRemoved -= HandleTilesRemoved;
        EventManager.OnTileSelected -= HandleTileSelected;
        EventManager.OnQuestClaimed -= HandleQuestClaimed;
        EventManager.OnBoughtItem -= HandleBoughtItem;
        EventManager.OnTransactionComplete -= HandleTransactionComplete;
        EventManager.OnQuestCompleted -= HandleQuestClaimed;

    }
    private void HandleTransactionComplete()
    {
        PlaySFX(successClip, 0.5f);
    }
    private void HandleBoughtItem(ShopItemSO itemSO)
    {
        PlaySFX(buyItemClip, 1);
    }
    private void HandleQuestClaimed(QuestDataSO quest)
    {
        PlaySFX(claimQuestClip, 1f);
    }
    private void HandleTileSelected(Tile tile)
    {
        if (tile == null) return;
        PlaySFX(clickSoundClip, 1.3f);
    }
    private void HandleTilesRemoved(TileDataSO tile)
    {
        PlaySFX(tileRemoveClip, 1f);
    }
    private void HandlePlayerLost()
    {
        PlayLose();
    }

    private void HandlePlayerWon()
    {
        PlayWinClip();
    }
    public void PlayWinClip(float volume = 1f)
    {
        if (!sfxOn || winClip == null) return;
        sfxSource.PlayOneShot(winClip, volume);
    }

    public void PlayLose(float volume = 1f)
    {
        if (!sfxOn || loseClip == null) return;
        sfxSource.PlayOneShot(loseClip, volume);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 0.73f)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;

        if (musicOn)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusic(bool on)
    {
        musicOn = on;
        PlayerPrefs.SetInt("music_on", on ? 1 : 0);

        musicSource.mute = !on;

        if (on && !musicSource.isPlaying && backgroundMusics.Count > 0)
        {
            PlayMusic(backgroundMusics[Random.Range(0, backgroundMusics.Count)]);
        }
        else if (!on)
        {
            musicSource.Pause();
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
        PlayerPrefs.SetInt("sfs_on", on ? 1 : 0);
    }

    public void Vibrate(float duration = 0.1f)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (vibrationOn)
            Handheld.Vibrate();
#endif
    }

    public void SetVibration(bool on)
    {
        vibrationOn = on;
    }
}
