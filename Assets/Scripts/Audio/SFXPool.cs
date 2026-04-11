using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXPool
{
    private readonly Queue<AudioSource> sources;
    private readonly Transform parent;
    private readonly AudioMixerGroup output;

    public SFXPool(Transform parent, int size, AudioMixerGroup output = null)
    {
        this.parent = parent;
        this.output = output;
        sources = new Queue<AudioSource>(size);

        for (int i = 0; i < size; i++)
            sources.Enqueue(CreateSource(i));
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource src = sources.Dequeue();
        src.PlayOneShot(clip, volume);
        sources.Enqueue(src);
    }

    public void StopAll()
    {
        foreach (AudioSource src in sources)
            src.Stop();
    }

    private AudioSource CreateSource(int index)
    {
        var go = new GameObject($"SfxSource_{index}");
        go.transform.SetParent(parent, false);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;
        src.outputAudioMixerGroup = output;
        return src;
    }
}