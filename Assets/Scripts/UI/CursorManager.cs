using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[NoAutoCreate]
public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Image defaultImage;
    [SerializeField] private Image hoverImage;
    private RectTransform cursorRT;
    private Canvas cursorCanvas;
    private Camera cam;

    protected override void Awake()
    {
        base.Awake();
        cursorCanvas = GetComponentInChildren<Canvas>();
        Cursor.visible = false;
        if (hoverImage.transform.parent != defaultImage.transform)
            hoverImage.transform.SetParent(defaultImage.transform);
        cursorRT = defaultImage.GetComponent<RectTransform>();
        AssignCamera();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        MatchManager.OnCountdownStarted += HideCursor;
        MatchManager.OnMatchEnded += ShowCursor;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignCamera();
    }

    private void OnDestroy()
    {
        MatchManager.OnCountdownStarted -= HideCursor;
        MatchManager.OnMatchEnded -= ShowCursor;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ShowCursor();
    }
    private void AssignCamera()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (!cursorCanvas.enabled || cam == null)
            return;
        cursorRT.position = Mouse.current.position.ReadValue();
        //Objetos 3D:
        if(Physics.Raycast(cam.ScreenPointToRay(cursorRT.position), out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out ICursorHoverTarget target) &&
                target.CanCauseHover())
            {
                UpdateState(true);
                return;
            }
        }
        //Objetos interfaz:
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
            { position = cursorRT.position }, results);
        foreach (var result in results)
        {
            Selectable s = result.gameObject.GetComponentInParent<Selectable>();
            if (s && s.interactable)
            {
                UpdateState(true);
                return;
            }
        }
        UpdateState(false);
    }

    private void UpdateState(bool hover)
    {
        defaultImage.enabled = !hover;
        hoverImage.enabled = hover;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cursorCanvas.enabled = false;
    }
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        cursorCanvas.enabled = true;
    }
}
