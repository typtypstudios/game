using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "FullScreenShaderEffect", menuName = "TypTyp/Effects/FullScreenShaderEffect")]
public class FullScreenShaderEffect : StatusEffectDefinition
{
    [SerializeField] private UniversalRendererData renderer;
    [SerializeField] private Material mat;
    [SerializeField] private VolumeProfile additionalVolume;
    private Volume volume;

    public override void OnActivate(Player target)
    {
        if (!target.IsOwner) return;
        foreach(var feature in renderer.rendererFeatures)
        {
            if (feature is FullScreenPassRendererFeature fullScreen && fullScreen.passMaterial == mat)
            {
                feature.SetActive(true);
                fullScreen.passMaterial.SetFloat("_StartTime", Time.time);
            }
        }
        if(additionalVolume)
        {
            volume = new GameObject().AddComponent<Volume>();
            volume.profile = additionalVolume;
        }
    }

    public override void OnDeactivate(Player target)
    {
        if (!target.IsOwner) return;
        foreach (var feature in renderer.rendererFeatures)
        {
            if (feature is FullScreenPassRendererFeature fullScreen && fullScreen.passMaterial == mat)
            {
                feature.SetActive(false);
            }
        }
        if (additionalVolume) Destroy(volume.gameObject);
    }

    public override string GetDefaultValue()
    {
        return "";
    }
}