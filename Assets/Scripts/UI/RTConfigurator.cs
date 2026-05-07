using System.Collections.Generic;
using UnityEngine;

public class RTConfigurator : MonoBehaviour
{
    [SerializeField] private RenderTexture[] rts;
    private Camera[] cameras;
    private int currentSize = 0;
    private Dictionary<RenderTexture, Vector2> initDimensions = new();

    void Awake()
    {
        cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ResizeRTs();
        foreach (var rt in rts)
        {
            initDimensions.Add(rt, new(rt.width, rt.height));
        }
    }

    private void OnDestroy()
    {
        foreach(var rt in rts)
        {
            rt.Release();
            rt.width = (int)initDimensions[rt].x;
            rt.height = (int)initDimensions[rt].y;
            rt.Create();
        }
    }

    public void ResizeRTs(int scaleFactor = 1)
    {
        if (scaleFactor == currentSize) return;
        foreach(var rt in rts)
        {
            rt.Release();
            rt.width = Screen.width * scaleFactor;
            rt.height = Screen.height * scaleFactor;
            rt.Create();
            foreach(var cam in cameras)
            {
                if (cam.targetTexture != rt) continue;
                cam.targetTexture = null; //Sin esto no funciona putísimo Unity
                cam.targetTexture = rt;
            }
        }
        currentSize = scaleFactor;
    }
}