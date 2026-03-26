using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RendererRestorer : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rend;
    private Dictionary<ScriptableRendererFeature, bool> enableList = new();

    private void Awake()
    {
        foreach(var feature in rend.rendererFeatures)
        {
            enableList[feature] = feature.isActive;
        }
    }

    private void OnDestroy()
    {
        foreach(var feature in rend.rendererFeatures)
        {
            feature.SetActive(enableList[feature]);
        }
    }
}
