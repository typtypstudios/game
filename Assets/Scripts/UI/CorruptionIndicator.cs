using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TypTyp;
using System.Collections;

[RequireComponent(typeof(Image))]
public class CorruptionIndicator : MonoBehaviour
{
    [Tooltip("Valores de intensidad asociados al mínimo de corrupción que necesitan." +
        "DEBEN IR EN ORDEN ASCENDENTE")]
    [SerializeField] private List<IndicatorValues> ranges = new();
    [Min(0)][SerializeField] private float stayTime = 1;
    [Min(0)][SerializeField] private float disappearSpeed = 1;
    private WaitForSeconds stayTimer;
    private Material mat;
    private float IndicatorIntensity 
    { 
        get { return mat.GetFloat("_Intensity"); }
        set { mat.SetFloat("_Intensity", value); }
    }

    private void Awake()
    {
        Image image = GetComponent<Image>();
        mat = new(image.material);
        image.material = mat;
        stayTimer = new(stayTime);
        GameUIConfigurator.OnUIConfigurated += Subscribe;
    }

    private void OnDestroy()
    {
        GameUIConfigurator.OnUIConfigurated -= Subscribe;
        Player.User.CurrentCorruption.OnValueChanged -= HandleCorruption;
    }

    private void Subscribe()
    {
        Player.User.CurrentCorruption.OnValueChanged += HandleCorruption;
    }

    private void HandleCorruption(float prevCorr, float newCorr)
    {
        float addedCorruption = newCorr - prevCorr;
        float normalizedCorruption = addedCorruption / Settings.Instance.MaxCorruption;
        for (int i = ranges.Count - 1; i >= 0; i--)
        {
            if (normalizedCorruption >= ranges[i].corruptionNeeded)
            {
                StopAllCoroutines();
                StartCoroutine(DisplayAnimation(ranges[i].intensity));
                return;
            }
        }
    }

    private void UpdateOffset() //Simplemente se cambia aleatoriamente para que no se vea igual siempre
    {
        Vector2 newOffset = new(UnityEngine.Random.Range(-100, 100), 
            UnityEngine.Random.Range(-100, 100));
        mat.SetVector("_Offset", newOffset);
    }

    IEnumerator DisplayAnimation(float intensity)
    {
        UpdateOffset();
        IndicatorIntensity = intensity;
        yield return stayTimer;
        while(intensity > 0)
        {
            intensity -= Time.deltaTime * disappearSpeed;
            IndicatorIntensity = intensity;
            yield return null;
        }
    }
}

[Serializable] 
public class IndicatorValues
{
    [Range(0, 1)] public float corruptionNeeded;
    [Range(0, 1)] public float intensity;
}
