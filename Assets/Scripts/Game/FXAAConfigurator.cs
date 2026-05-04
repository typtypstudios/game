using TypTyp;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class FXAAConfigurator : MonoBehaviour
{
    private UniversalAdditionalCameraData camData;
    
    void Awake()
    {
        camData = GetComponent<UniversalAdditionalCameraData>();
        camData.antialiasingQuality = AntialiasingQuality.High;
        GameSettings.OnAAUpdated += ConfigureFXAA;
        ConfigureFXAA();
    }

    private void OnDestroy()
    {
        GameSettings.OnAAUpdated -= ConfigureFXAA;
    }

    private void ConfigureFXAA()
    {
        camData.antialiasing = Settings.Instance.FXAA ? 
            AntialiasingMode.FastApproximateAntialiasing : AntialiasingMode.None;        
    }
}
