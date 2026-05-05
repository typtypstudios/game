using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTransitionManager : MonoBehaviour
{
    [Min(0)][field: SerializeField] public float TransitionTime { get; private set; } = 2;
    [SerializeField] private Material transitionMat;
    private bool blocked = false;
    private readonly Dictionary<object, Action> onStartedActions = new();
    private readonly Dictionary<object, Action> onDissolvedActions = new();
    private readonly Dictionary<object, Action> onEndedActions = new();
    private readonly Dictionary<object, Action> onCanceledActions = new();
    private object activeSender;
    public static event Action OnTransitionFinished;
    public static event Action OnDissolved;
    private float Dissolve
    {
        get { return transitionMat.GetFloat("_Dissolve"); }
        set { transitionMat.SetFloat("_Dissolve", Mathf.Clamp01(value)); }
    }

    private void OnDestroy() => Dissolve = 0;

    public void SetDissolve(float dissolve) => Dissolve = dissolve;

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

    public void SubscribeOnCanceled(object sender, Action action)
    {
        onCanceledActions[sender] = action;
    }

    public void PerformTransition(Canvas origin, Canvas dest, object sender, bool blockTransitioner, float time = -1)
    {
        if (blocked) return;
        if (activeSender != null && activeSender != sender && onCanceledActions.ContainsKey(activeSender))
            onCanceledActions[activeSender]?.Invoke();
        activeSender = sender;
        StopAllCoroutines();
        StartCoroutine(TransitionCoroutine(origin, dest, sender, blockTransitioner, time));
    }

    private IEnumerator TransitionCoroutine(Canvas origin, Canvas dest, object sender, bool block, float time = -1)
    {
        if (block)
        {
            blocked = true;
            origin.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        if (onStartedActions.ContainsKey(sender)) onStartedActions[sender]?.Invoke();
        float speed = 2 / (time == -1 ? TransitionTime : time);
        float dissolveValue = Dissolve; //Para no hacer gets constantes
        while (dissolveValue < 1)
        {
            dissolveValue += speed * Time.deltaTime;
            Dissolve = dissolveValue;
            yield return null;
        }
        origin.enabled = false;
        if(onDissolvedActions.ContainsKey(sender)) onDissolvedActions[sender]?.Invoke();
        OnDissolved?.Invoke();
        dest.enabled = true;
        dissolveValue = 1;
        while (dissolveValue > 0)
        {
            dissolveValue -= speed * Time.deltaTime;
            Dissolve = dissolveValue;
            yield return null;
        }
        if (onEndedActions.ContainsKey(sender)) onEndedActions[sender]?.Invoke();
        if (block)
        {
            dest.GetComponent<CanvasGroup>().blocksRaycasts = true;
            blocked = false;
        }
        activeSender = null;
        OnTransitionFinished?.Invoke();
    }
}