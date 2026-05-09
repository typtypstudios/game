using System;
using UnityEngine;

public class TurnPageEffect : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.8f;
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
        transitionManager.SubscribeOnStarted(this, () => AudioManager.Instance.PlayUI(UISound.DissolveOut));
        transitionManager.SubscribeOnDissolved(this, () =>
        {
            OnBlankPage?.Invoke();
            AudioManager.Instance.PlayUI(UISound.DissolveIn);
        });
        transitionManager.SubscribeOnEnded(this, OnTurnEnded);
    }

    public void InitializePages(Transform[] pages) => pagesTransform = pages;

    public void TurnPage()
    {
        parentCanvas.gameObject.layer = LayerMask.NameToLayer("TurnPageStay");
        foreach (var page in pagesTransform)
        {
            if (!page.gameObject.TryGetComponent(out Canvas _)) page.gameObject.AddComponent<Canvas>();
            page.gameObject.layer = LayerMask.NameToLayer("UI");
        }
        transitionManager.PerformTransition(parentCanvas, parentCanvas, this, false, transitionTime);
    }

    private void OnTurnEnded()
    {
        parentCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
        OnTurnFinished?.Invoke();
        foreach (var page in pagesTransform)
            Destroy(page.gameObject.GetComponent<Canvas>());
    }
}
