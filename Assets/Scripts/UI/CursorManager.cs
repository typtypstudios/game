using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[NoAutoCreate]
public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Image hoverImage;
    private RectTransform hoverRT;
    private Canvas hoverCanvas;
    private Camera cam;

    protected override void Awake()
    {
        base.Awake();
        hoverCanvas = GetComponentInChildren<Canvas>();
        hoverRT = hoverImage.GetComponent<RectTransform>();
        AssignCamera();
        SceneManager.sceneLoaded += OnSceneLoaded;
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
        if (!hoverCanvas.enabled || cam == null)
            return;
        hoverRT.position = Mouse.current.position.ReadValue();
        if (NavigationController.Navigating)
        {
            UpdateState(false);
            return;
        }
        //Objetos 3D:
        if(Physics.Raycast(cam.ScreenPointToRay(hoverRT.position), out RaycastHit hit))
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
            { position = hoverRT.position }, results);
        if (results.Count > 0)
        {
            Selectable s = results[0].gameObject.GetComponentInParent<Selectable>();
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
        Cursor.visible = !hover;
        hoverImage.enabled = hover;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        hoverCanvas.enabled = false;
    }
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        hoverCanvas.enabled = true;
    }
}