using System;
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

public class AudioManager : Singleton<AudioManager>
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;

    [Header("Snapshots")]
    [SerializeField] private AudioMixerSnapshot menuSnapshot;
    [SerializeField] private AudioMixerSnapshot gameSnapshot;

    [Header("SFX Pool")]
    [SerializeField, Min(1)] private int sfxPoolSize = 12;

    [Header("UI Sounds")]
    [SerializeField] private UISoundEntry[] uiSounds;

    private SFXPool sfxPool;
    private SFXPool uiPool;
    private MusicPlayer music;
    private Dictionary<UISound, UISoundEntry> uiMap;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        sfxPool = new SFXPool(transform, sfxPoolSize, sfxGroup);
        uiPool = new SFXPool(transform, 4, uiGroup);
        music = new MusicPlayer(transform, this, musicGroup);

        BuildUiMap();
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

    #region Music

    public void PlayMusic(AudioClip clip, float fadeDuration = 1f, float volume = 1f)
        => music.Play(clip, fadeDuration, volume);

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

    #endregion
}