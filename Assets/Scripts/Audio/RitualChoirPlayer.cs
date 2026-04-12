using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class RitualChoirPlayer : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip choirClip;
    [SerializeField] private AudioClip coughClip;

    [Header("Choir")]
    [SerializeField, Range(0f, 1f)] private float minVolume = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxVolume = 1f;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 1.3f;
    [SerializeField] private float crossfadeDuration = 1.5f;
    [SerializeField] private float pauseFadeDuration = 0.3f;
    [SerializeField] private float resumeFadeDuration = 0.4f;
    [SerializeField] private float matchEndFadeDuration = 1.5f;
    [SerializeField] private float idleTimeout = 2f;
    [SerializeField] private float idleFadeDuration = 0.6f;

    [Header("Reverb")]
    [SerializeField] private string reverbParam = "ChoirReverb";
    [SerializeField] private float minReverb = -10000f;
    [SerializeField] private float maxReverb = 1000f;

    [Header("Cough")]
    [SerializeField, Range(0f, 1f)] private float coughVolume = 1f;

    private RitualManager ritualManager;
    private AudioSource choirA;
    private AudioSource choirB;
    private AudioSource coughSource;
    private AudioSource activeChoir;
    private bool isCoughing = false;
    private bool waitingForNextCorrect = false;
    private bool isPlaying = false;
    private bool isIdle = false;
    private float timeSinceLastCorrect = 0f;
    private float currentTargetVolume;
    private Coroutine crossfadeRoutine;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        choirA = CreateChoirSource();
        choirB = CreateChoirSource();
        activeChoir = choirA;

        coughSource = gameObject.AddComponent<AudioSource>();
        coughSource.clip = coughClip;
        coughSource.loop = false;
        coughSource.playOnAwake = false;
        coughSource.volume = coughVolume;

        if (AudioManager.Instance != null)
        {
            AudioMixerGroup group = AudioManager.Instance.ChoirGroup;
            choirA.outputAudioMixerGroup = group;
            choirB.outputAudioMixerGroup = group;
            coughSource.outputAudioMixerGroup = group;
        }

        currentTargetVolume = minVolume;
    }

    private AudioSource CreateChoirSource()
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = choirClip;
        src.loop = false;
        src.playOnAwake = false;
        src.volume = 0f;
        src.pitch = minPitch;
        return src;
    }

    private void OnEnable()
    {
        MatchManager.OnMatchEnded += HandleMatchEnded;
    }

    public void Bind(RitualManager manager)
    {
        if (ritualManager != null)
        {
            ritualManager.OnCorrectChar -= HandleCorrect;
            ritualManager.OnWrongChar -= HandleWrong;
            ritualManager.OnProgressUpdated -= HandleProgress;
        }

        ritualManager = manager;
        if (ritualManager == null) return;

        ritualManager.OnCorrectChar += HandleCorrect;
        ritualManager.OnWrongChar += HandleWrong;
        ritualManager.OnProgressUpdated += HandleProgress;
    }

    private void OnDisable()
    {
        if (ritualManager != null)
        {
            ritualManager.OnCorrectChar -= HandleCorrect;
            ritualManager.OnWrongChar -= HandleWrong;
            ritualManager.OnProgressUpdated -= HandleProgress;
        }
        MatchManager.OnMatchEnded -= HandleMatchEnded;
        StopAllCoroutines();
    }

    private void Update()
    {
        // Idle check: si lleva demasiado sin tecla correcta, pausar el coro con fade out
        if (isPlaying && !isIdle && !isCoughing && !waitingForNextCorrect)
        {
            timeSinceLastCorrect += Time.deltaTime;
            if (timeSinceLastCorrect >= idleTimeout)
            {
                isIdle = true;
                if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(FadeOutAndPause(activeChoir, idleFadeDuration));
            }
        }

        if (!isPlaying || isIdle || isCoughing || waitingForNextCorrect) return;

        float remaining = activeChoir.clip.length - activeChoir.time;
        if (remaining <= crossfadeDuration && crossfadeRoutine == null)
        {
            AudioSource next = activeChoir == choirA ? choirB : choirA;
            if (!next.isPlaying)
            {
                next.pitch = activeChoir.pitch;
                next.volume = 0f;
                next.time = 0f;
                next.Play();
                crossfadeRoutine = StartCoroutine(Crossfade(activeChoir, next, crossfadeDuration));
                activeChoir = next;
            }
        }
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to, float duration)
    {
        float t = 0f;
        float startFrom = from.volume;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            from.volume = Mathf.Lerp(startFrom, 0f, k);
            to.volume = Mathf.Lerp(0f, currentTargetVolume, k);
            yield return null;
        }
        from.Stop();
        from.volume = 0f;
        to.volume = currentTargetVolume;
        crossfadeRoutine = null;
    }

    private IEnumerator FadeOutAndPause(AudioSource src, float duration)
    {
        float startVolume = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            src.volume = Mathf.Lerp(startVolume, 0f, k);
            yield return null;
        }
        src.volume = 0f;
        src.Pause();
        fadeRoutine = null;
    }

    private IEnumerator FadeIn(AudioSource src, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            src.volume = Mathf.Lerp(0f, currentTargetVolume, k);
            yield return null;
        }
        src.volume = currentTargetVolume;
        fadeRoutine = null;
    }

    private IEnumerator FadeOutAndStopAll(float duration)
    {
        float startA = choirA.volume;
        float startB = choirB.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            choirA.volume = Mathf.Lerp(startA, 0f, k);
            choirB.volume = Mathf.Lerp(startB, 0f, k);
            yield return null;
        }
        choirA.Stop();
        choirB.Stop();
        choirA.volume = 0f;
        choirB.volume = 0f;
        fadeRoutine = null;
    }

    private void HandleCorrect()
    {
        timeSinceLastCorrect = 0f;

        if (isCoughing) return;

        if (waitingForNextCorrect)
        {
            waitingForNextCorrect = false;
            activeChoir.UnPause();
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeIn(activeChoir, resumeFadeDuration));
            return;
        }

        if (isIdle)
        {
            isIdle = false;
            activeChoir.UnPause();
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeIn(activeChoir, resumeFadeDuration));
            return;
        }

        if (!isPlaying)
        {
            isPlaying = true;
            activeChoir.volume = 0f;
            activeChoir.Play();
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeIn(activeChoir, resumeFadeDuration));
        }
    }

    private void HandleWrong()
    {
        if (isCoughing) return;

        coughSource.Play();
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutAndPause(activeChoir, pauseFadeDuration));
        isCoughing = true;
        isIdle = false;
        StartCoroutine(WaitForCoughEnd());
    }

    private IEnumerator WaitForCoughEnd()
    {
        yield return new WaitWhile(() => coughSource.isPlaying);
        isCoughing = false;
        waitingForNextCorrect = true;
    }

    private void HandleMatchEnded()
    {
        StopAllCoroutines();
        crossfadeRoutine = null;
        fadeRoutine = StartCoroutine(FadeOutAndStopAll(matchEndFadeDuration));
        isPlaying = false;
        isCoughing = false;
        isIdle = false;
        waitingForNextCorrect = false;
    }

    private void HandleProgress(float progress)
    {
        float p = Mathf.Clamp01(progress);
        currentTargetVolume = Mathf.Lerp(minVolume, maxVolume, p);
        float pitch = Mathf.Lerp(minPitch, maxPitch, p);

        if (crossfadeRoutine == null && fadeRoutine == null && isPlaying && !isIdle && !isCoughing && !waitingForNextCorrect)
            activeChoir.volume = currentTargetVolume;

        activeChoir.pitch = pitch;

        if (AudioManager.Instance != null && !string.IsNullOrEmpty(reverbParam))
        {
            float reverbDb = Mathf.Lerp(minReverb, maxReverb, p);
            AudioManager.Instance.SetMixerParam(reverbParam, reverbDb);
        }
    }
}