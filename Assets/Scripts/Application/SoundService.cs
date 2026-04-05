// Scripts/Application/SoundService.cs
using System.Collections.Generic;
using UnityEngine;

public class SoundService
{
    private readonly ISaveService save;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private bool musicOn;
    private bool sfxOn;
    private bool vibrationOn;

    // Clip registry — đăng ký từ SoundServiceInstaller (MonoBehaviour)
    private readonly Dictionary<SoundID, AudioClip> clips = new();
    private readonly List<AudioClip> backgroundMusics = new();

    public bool MusicOn => musicOn;
    public bool SfxOn => sfxOn;
    public bool VibrationOn => vibrationOn;

    public SoundService(ISaveService save)
    {
        this.save = save;
        musicOn = save.GetInt(SaveKeys.Audio.MusicOn, 1) == 1;
        sfxOn = save.GetInt(SaveKeys.Audio.SfxOn, 1) == 1;
        vibrationOn = save.GetInt(SaveKeys.Audio.VibrationOn, 1) == 1;

        // Subscribe events
        EventBus<PlaySFXEvent>.Subscribe(OnPlaySFX);
        EventBus<SetMusicEvent>.Subscribe(OnSetMusic);
        EventBus<SetSFXEvent>.Subscribe(OnSetSFX);
        EventBus<PlayerWonEvent>.Subscribe(_ => PlaySFX(SoundID.Win));
        EventBus<PlayerLostEvent>.Subscribe(_ => PlaySFX(SoundID.Lose));
        EventBus<TilesRemovedEvent>.Subscribe(_ => PlaySFX(SoundID.TileRemove));
        EventBus<TileSelectedEvent>.Subscribe(_ => PlaySFX(SoundID.Click));
        EventBus<QuestClaimedEvent>.Subscribe(_ => PlaySFX(SoundID.ClaimQuest));
        EventBus<ItemPurchasedEvent>.Subscribe(_ => PlaySFX(SoundID.BuyItem));
        EventBus<TransactionCompleteEvent>.Subscribe(_ => PlaySFX(SoundID.Success));
    }

    // ── Setup (gọi từ SoundServiceInstaller) ─────────
    public void SetAudioSources(AudioSource music, AudioSource sfx)
    {
        musicSource = music;
        sfxSource = sfx;
        musicSource.mute = !musicOn;
        sfxSource.mute = !sfxOn;
    }

    public void RegisterClip(SoundID id, AudioClip clip)
    {
        if (clip != null)
            clips[id] = clip;
    }

    public void RegisterBackgroundMusics(List<AudioClip> list)
    {
        backgroundMusics.Clear();
        backgroundMusics.AddRange(list);

        if (musicOn && backgroundMusics.Count > 0)
            PlayRandomMusic();
    }

    // ── Public API ────────────────────────────────────
    public void PlaySFX(SoundID id, float volume = 1f)
    {
        if (!sfxOn) return;
        if (!clips.TryGetValue(id, out var clip)) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayRandomMusic()
    {
        if (backgroundMusics.Count == 0) return;
        var clip = backgroundMusics[Random.Range(0, backgroundMusics.Count)];
        PlayMusic(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 0.73f)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        if (musicOn) musicSource.Play();
    }

    public void StopMusic() => musicSource?.Stop();

    public void SetMusic(bool on)
    {
        musicOn = on;
        musicSource.mute = !on;
        save.SetInt(SaveKeys.Audio.MusicOn, on ? 1 : 0);
        save.Save();

        if (on && musicSource != null && !musicSource.isPlaying)
            PlayRandomMusic();
        else if (!on)
            musicSource?.Pause();
    }

    public void SetSFX(bool on)
    {
        sfxOn = on;
        sfxSource.mute = !on;
        save.SetInt(SaveKeys.Audio.SfxOn, on ? 1 : 0);
        save.Save();
    }

    public void SetVibration(bool on)
    {
        vibrationOn = on;
        save.SetInt(SaveKeys.Audio.VibrationOn, on ? 1 : 0);
        save.Save();
    }

    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (vibrationOn) Handheld.Vibrate();
#endif
    }

    // ── Event handlers ────────────────────────────────
    private void OnPlaySFX(PlaySFXEvent e) => PlaySFX(e.id);
    private void OnSetMusic(SetMusicEvent e) => SetMusic(e.on);
    private void OnSetSFX(SetSFXEvent e) => SetSFX(e.on);

    public void Dispose()
    {
        EventBus<PlaySFXEvent>.Unsubscribe(OnPlaySFX);
        EventBus<SetMusicEvent>.Unsubscribe(OnSetMusic);
        EventBus<SetSFXEvent>.Unsubscribe(OnSetSFX);
    }
}