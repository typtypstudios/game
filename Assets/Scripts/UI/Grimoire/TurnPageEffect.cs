using System;
using UnityEngine;

public class TurnPageEffect : MonoBehaviour
{
    [SerializeField] private Transform[] pagesTransform;
    private Canvas parentCanvas;
    private CanvasTransitionManager transitionManager;
    public event Action OnBlankPage;
    public event Action OnTurnFinished;

    private void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        transitionManager = FindFirstObjectByType<CanvasTransitionManager>();
        if (!transitionManager) Debug.LogError("Error: no hay CanvasTransitionManager en la escena.");
        transitionManager.SubscribeOnDissolved(this, () => OnBlankPage?.Invoke());
        transitionManager.SubscribeOnEnded(this, OnTurnEnded);
        transitionManager.SubscribeOnCanceled(this, OnCanceled);
    }

    public void TurnPage()
    {
        parentCanvas.gameObject.layer = 0;
        foreach (var page in pagesTransform)
        {
            if(!page.gameObject.TryGetComponent(out Canvas _)) page.gameObject.AddComponent<Canvas>();
            page.gameObject.layer = LayerMask.NameToLayer("UI");
        }
        transitionManager.PerformTransition(parentCanvas, parentCanvas, this, false);
    }

    private void OnTurnEnded()
    {
        parentCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
        OnTurnFinished?.Invoke();
        foreach (var page in pagesTransform)
            Destroy(page.gameObject.GetComponent<Canvas>());
    }

    private void OnCanceled()
    {
        OnBlankPage?.Invoke(); //Para aplicar cambios
        OnTurnEnded();
    }
}
