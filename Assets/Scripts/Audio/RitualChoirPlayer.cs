using System.Collections;
using UnityEngine;

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
    private float currentTargetVolume;
    private Coroutine crossfadeRoutine;

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
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!isPlaying || isCoughing || waitingForNextCorrect) return;

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

    private void HandleCorrect()
    {
        if (isCoughing) return;

        if (waitingForNextCorrect)
        {
            waitingForNextCorrect = false;
            activeChoir.UnPause();
            return;
        }

        if (!isPlaying)
        {
            isPlaying = true;
            activeChoir.volume = currentTargetVolume;
            activeChoir.Play();
        }
    }

    private void HandleWrong()
    {
        activeChoir.Pause();
        coughSource.Stop();
        coughSource.Play();
        isCoughing = true;
        StartCoroutine(WaitForCoughEnd());
    }

    private IEnumerator WaitForCoughEnd()
    {
        yield return new WaitWhile(() => coughSource.isPlaying);
        isCoughing = false;
        waitingForNextCorrect = true;
    }

    private void HandleProgress(float progress)
    {
        float p = Mathf.Clamp01(progress);
        currentTargetVolume = Mathf.Lerp(minVolume, maxVolume, p);
        float pitch = Mathf.Lerp(minPitch, maxPitch, p);

        if (crossfadeRoutine == null && isPlaying && !isCoughing && !waitingForNextCorrect)
            activeChoir.volume = currentTargetVolume;

        activeChoir.pitch = pitch;
    }
}