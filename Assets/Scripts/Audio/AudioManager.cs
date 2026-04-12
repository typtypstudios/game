using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class UISoundEntry
{
    public UISound id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

[Serializable]
public class MusicEntry
{
    public MusicTrack id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public float fadeDuration = 1.5f;
}

[Serializable]
public class CountdownSoundEntry
{
    public CountdownSound id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

[Serializable]
public class CultSoundEntry
{
    public CultSound id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

[Serializable]
public class GameSoundEntry
{
    public GameSound id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class AudioManager : Singleton<AudioManager>
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    [SerializeField] private AudioMixerGroup choirGroup;
    [SerializeField] private AudioMixerGroup keyGroup;

    [Header("Snapshots")]
    [SerializeField] private AudioMixerSnapshot menuSnapshot;
    [SerializeField] private AudioMixerSnapshot gameSnapshot;

    [Header("SFX Pool")]
    [SerializeField, Min(1)] private int sfxPoolSize = 12;

    [Header("UI Sounds")]
    [SerializeField] private UISoundEntry[] uiSounds;

    [Header("Music Tracks")]
    [SerializeField] private MusicEntry[] musicTracks;

    [Header("Countdown Sounds")]
    [SerializeField] private CountdownSoundEntry[] countdownSounds;

    [Header("Cult Sounds")]
    [SerializeField] private CultSoundEntry[] cultSounds;

    [Header("Game Sounds")]
    [SerializeField] private GameSoundEntry[] gameSounds;

    private SFXPool sfxPool;
    private SFXPool uiPool;
    private MusicPlayer music;
    private Dictionary<UISound, UISoundEntry> uiMap;
    private Dictionary<MusicTrack, MusicEntry> musicMap;
    private Dictionary<CountdownSound, CountdownSoundEntry> countdownMap;
    private Dictionary<CultSound, CultSoundEntry> cultMap;
    private Dictionary<GameSound, GameSoundEntry> gameMap;

    public AudioMixerGroup ChoirGroup => choirGroup;
    public AudioMixerGroup KeyGroup => keyGroup;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        sfxPool = new SFXPool(transform, sfxPoolSize, sfxGroup);
        uiPool = new SFXPool(transform, 4, uiGroup);
        music = new MusicPlayer(transform, this, musicGroup);

        BuildUiMap();
        BuildMusicMap();
        BuildCountdownMap();
        BuildCultMap();
        BuildGameMap();
    }

    private void BuildUiMap()
    {
        uiMap = new Dictionary<UISound, UISoundEntry>();
        if (uiSounds == null) return;
        foreach (var entry in uiSounds)
        {
            if (entry == null || entry.clip == null) continue;
            uiMap[entry.id] = entry;
        }
    }

    private void BuildMusicMap()
    {
        musicMap = new Dictionary<MusicTrack, MusicEntry>();
        if (musicTracks == null) return;
        foreach (var entry in musicTracks)
        {
            if (entry == null || entry.clip == null) continue;
            musicMap[entry.id] = entry;
        }
    }

    private void BuildCountdownMap()
    {
        countdownMap = new Dictionary<CountdownSound, CountdownSoundEntry>();
        if (countdownSounds == null) return;
        foreach (var entry in countdownSounds)
        {
            if (entry == null || entry.clip == null) continue;
            countdownMap[entry.id] = entry;
        }
    }

    private void BuildCultMap()
    {
        cultMap = new Dictionary<CultSound, CultSoundEntry>();
        if (cultSounds == null) return;
        foreach (var entry in cultSounds)
        {
            if (entry == null || entry.clip == null) continue;
            cultMap[entry.id] = entry;
        }
    }

    private void BuildGameMap()
    {
        gameMap = new Dictionary<GameSound, GameSoundEntry>();
        if (gameSounds == null) return;
        foreach (var entry in gameSounds)
        {
            if (entry == null || entry.clip == null) continue;
            gameMap[entry.id] = entry;
        }
    }

    #region Music

    public void PlayMusic(MusicTrack track)
    {
        if (track == MusicTrack.None) { StopMusic(); return; }
        if (musicMap.TryGetValue(track, out var entry))
            music.Play(entry.clip, entry.fadeDuration, entry.volume);
        else
            Debug.LogWarning($"[AudioManager] MusicTrack '{track}' no está mapeado.");
    }

    public void StopMusic(float fadeDuration = 1f)
        => music.Stop(fadeDuration);

    #endregion

    #region SFX 2D

    public void PlaySFX(AudioClip clip, float volume = 1f)
        => sfxPool.PlayOneShot(clip, volume);

    public void PlayUI(UISound id)
    {
        if (id == UISound.None) return;
        if (uiMap.TryGetValue(id, out var entry))
            uiPool.PlayOneShot(entry.clip, entry.volume);
        else
            Debug.LogWarning($"[AudioManager] UISound '{id}' no está mapeado.");
    }

    public void PlayCountdown(CountdownSound id)
    {
        if (id == CountdownSound.None) return;
        if (countdownMap.TryGetValue(id, out var entry))
            uiPool.PlayOneShot(entry.clip, entry.volume);
        else
            Debug.LogWarning($"[AudioManager] CountdownSound '{id}' no está mapeado.");
    }

    public void PlayCult(CultSound id)
    {
        if (id == CultSound.None) return;
        if (cultMap.TryGetValue(id, out var entry))
            sfxPool.PlayOneShot(entry.clip, entry.volume);
        else
            Debug.LogWarning($"[AudioManager] CultSound '{id}' no está mapeado.");
    }

    public void PlayGame(GameSound id)
    {
        if (id == GameSound.None) return;
        if (gameMap.TryGetValue(id, out var entry))
            sfxPool.PlayOneShot(entry.clip, entry.volume);
        else
            Debug.LogWarning($"[AudioManager] GameSound '{id}' no está mapeado.");
    }

    public void PlayGame(GameSound id, float delay)
    {
        if (id == GameSound.None) return;
        if (delay <= 0f) { PlayGame(id); return; }
        StartCoroutine(PlayGameDelayed(id, delay));
    }

    private IEnumerator PlayGameDelayed(GameSound id, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayGame(id);
    }

    public void StopAllSFX()
    {
        sfxPool.StopAll();
        uiPool.StopAll();
    }

    #endregion

    #region SFX 3D posicional

    public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return;

        var go = new GameObject($"SfxAt_{clip.name}");
        go.transform.position = position;
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.spatialBlend = spatialBlend;
        src.outputAudioMixerGroup = sfxGroup;
        src.Play();
        Destroy(go, clip.length + 0.1f);
    }

    #endregion

    #region Snapshots / Mixer

    public void TransitionToMenu(float duration = 1f)
    {
        if (menuSnapshot != null) menuSnapshot.TransitionTo(duration);
    }

    public void TransitionToGame(float duration = 1f)
    {
        if (gameSnapshot != null) gameSnapshot.TransitionTo(duration);
    }

    public void SetBusVolume(string exposedParam, float normalized01)
    {
        if (mixer == null || string.IsNullOrEmpty(exposedParam)) return;
        float db = normalized01 <= 0.0001f ? -80f : Mathf.Log10(normalized01) * 20f;
        mixer.SetFloat(exposedParam, db);
    }

    public void SetMixerParam(string exposedParam, float value)
    {
        if (mixer == null || string.IsNullOrEmpty(exposedParam)) return;
        mixer.SetFloat(exposedParam, value);
    }

    #endregion
}