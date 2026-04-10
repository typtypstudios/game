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
    }

    //Cuidado con interacciones mientras trasnición, con cartas y otra interfaz.
    public void TurnPage()
    {
        parentCanvas.gameObject.layer = 0;
        foreach (var page in pagesTransform)
        {
            page.gameObject.AddComponent<Canvas>();
            page.gameObject.layer = LayerMask.NameToLayer("UI");
        }
        transitionManager.PerformTransition(parentCanvas, parentCanvas, this);
    }

    private void OnTurnEnded()
    {
        parentCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
        OnTurnFinished?.Invoke();
        foreach (var page in pagesTransform)
            Destroy(page.gameObject.GetComponent<Canvas>());
    }
}
