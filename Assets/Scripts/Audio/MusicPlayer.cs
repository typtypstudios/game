using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer
{
    private readonly AudioSource source;
    private readonly MonoBehaviour runner;
    private Coroutine fadeRoutine;

    public MusicPlayer(Transform parent, MonoBehaviour runner, AudioMixerGroup output = null)
    {
        this.runner = runner;

        var go = new GameObject("MusicSource");
        go.transform.SetParent(parent, false);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = output;
    }

    public void Play(AudioClip clip, float fadeDuration = 0f, float targetVolume = 1f)
    {
        if (clip == null) return;
        StopFade();

        if (fadeDuration <= 0f)
        {
            source.clip = clip;
            source.volume = targetVolume;
            source.Play();
            return;
        }

        fadeRoutine = runner.StartCoroutine(FadeIn(clip, fadeDuration, targetVolume));
    }

    public void Stop(float fadeDuration = 0f)
    {
        StopFade();

        if (fadeDuration <= 0f)
        {
            source.Stop();
            return;
        }

        fadeRoutine = runner.StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeIn(AudioClip clip, float duration, float targetVolume)
    {
        source.clip = clip;
        source.volume = 0f;
        source.Play();

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        source.volume = startVolume;
        source.Stop();
    }

    private void StopFade()
    {
        if (fadeRoutine != null)
        {
            runner.StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
    }
}