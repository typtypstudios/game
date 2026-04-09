using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTransitionManager : MonoBehaviour
{
    [Min(0)][field: SerializeField] public float TransitionTime { get; private set; } = 2;
    [SerializeField] private Material transitionMat;
    [SerializeField] private RenderTexture transitionTexture;
    private Camera uiCam;
    private bool performingTransition = false;
    private readonly Dictionary<object, Action> onStartedActions = new();
    private readonly Dictionary<object, Action> onDissolvedActions = new();
    private readonly Dictionary<object, Action> onEndedActions = new();
    private float Dissolve
    {
        get { return transitionMat.GetFloat("_Dissolve"); }
        set { transitionMat.SetFloat("_Dissolve", Mathf.Clamp01(value)); }
    }

    private void Awake()
    {
        uiCam = GameObject.FindGameObjectWithTag("UICam").GetComponent<Camera>();
        Dissolve = 1;
    }

    public void SubscribeOnStarted(object sender, Action action)
    {
        onStartedActions[sender] = action;
    }

    public void SubscribeOnDissolved(object sender, Action action)
    {
        onDissolvedActions[sender] = action;
    }

    public void SubscribeOnEnded(object sender, Action action)
    {
        onEndedActions[sender] = action;
    }

    public void PerformTransition(Canvas origin, Canvas dest, object sender)
    {
        if (performingTransition) return;
        StopAllCoroutines();
        StartCoroutine(TransitionCoroutine(origin, dest, sender));
    }

    private IEnumerator TransitionCoroutine(Canvas origin, Canvas dest, object sender)
    {
        performingTransition = true;
        if (onStartedActions.ContainsKey(sender)) onStartedActions[sender]?.Invoke();
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        uiCam.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
        origin.GetComponent<CanvasGroup>().blocksRaycasts = false;
        float speed = 2 / TransitionTime;
        float dissolveValue = Dissolve; //Para no hacer gets constantes
        while (dissolveValue < 1)
        {
            dissolveValue += speed * Time.deltaTime;
            Dissolve = dissolveValue;
            yield return null;
        }
        origin.enabled = false;
        if(onDissolvedActions.ContainsKey(sender)) onDissolvedActions[sender]?.Invoke();
        dest.enabled = true;
        dissolveValue = 1;
        while (dissolveValue > 0)
        {
            dissolveValue -= speed * Time.deltaTime;
            Dissolve = dissolveValue;
            yield return null;
        }
        dest.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
        uiCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        if (onEndedActions.ContainsKey(sender)) onEndedActions[sender]?.Invoke();
        performingTransition = false;
    }
}