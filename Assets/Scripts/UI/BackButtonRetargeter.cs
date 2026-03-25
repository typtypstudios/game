using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButtonRetargeter : MonoBehaviour
{
    [SerializeField] private Canvas defaultTarget;
    [SerializeField] private bool resetOnClick = true;
    private Canvas targetCanvas;

    private void Awake()
    {
        targetCanvas = defaultTarget;
    }

    public void ResetTarget() => targetCanvas = defaultTarget;

    public void SetTarget(Canvas target) => targetCanvas = target;

    public void OpenTarget()
    {
        targetCanvas.enabled = true;
        if (resetOnClick) ResetTarget();
    }
}
