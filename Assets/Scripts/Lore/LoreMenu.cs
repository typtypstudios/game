using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas))]
public class LoreMenu : AInputListener, INavigationCtxReceiver, INavigationLeaveReceiver
{
    [SerializeField] private TMP_Text tmp;
    [TextArea][SerializeField] private string[] texts;
    [Min(0)][SerializeField] private float textAppearSpeed = 1;
    [SerializeField] private InputActionReference clickAction;
    private int textIdx = 0;
    private int charIdx = 0;
    private string currentText;
    private bool isFocused = false;
    private const float CHAR_APPEAR_INTERVAL = 0.1f;

    private void Awake()
    {
        clickAction.action.performed += ProcessInput;
    }

    private void OnDestroy()
    {
        clickAction.action.performed -= ProcessInput;
        CanvasTransitionManager.OnTransitionFinished -= DisplayText;
    }

    protected override void ProcessInput(char _) => OnInteraction();

    private void ProcessInput(InputAction.CallbackContext _) => OnInteraction();

    private void OnInteraction()
    {
        if (!isFocused) return;
        StopAllCoroutines();
        if (charIdx < currentText.Length)
        {
            charIdx = currentText.Length;
            UpdateView();
        }
        else
        {
            textIdx++;
            DisplayText();
        }
    }

    public void ReceiveContext(Screens prevScreen, bool isGoingBack, GameObject sender = null)
    {
        textIdx = 0;
        charIdx = 0;
        CanvasTransitionManager.OnTransitionFinished += DisplayText;
        isFocused = true;
    }

    public void OnLeave()
    {
        CanvasTransitionManager.OnTransitionFinished -= DisplayText;
        StopAllCoroutines();
        isFocused = false;
        tmp.text = string.Empty;
    }

    private void DisplayText()
    {
        currentText = textIdx < texts.Length ? texts[textIdx] : "...";
        charIdx = 0;
        StopAllCoroutines();
        StartCoroutine(DisplayTextCoroutine());
    }

    private void UpdateView()
    {
        tmp.text = currentText[..charIdx] + "<alpha=#00>" + currentText[charIdx..];
    }

    IEnumerator DisplayTextCoroutine()
    {
        while(charIdx <= currentText.Length)
        {
            yield return new WaitForSeconds(CHAR_APPEAR_INTERVAL * textAppearSpeed);
            UpdateView();
            charIdx++;
        }
    }
}