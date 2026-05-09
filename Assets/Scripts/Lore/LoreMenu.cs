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
    [Header("On end messages")]
    [SerializeField] private int numInteractionsToAnger = 10;
    [SerializeField] private string endMessage = "...";
    [SerializeField] private string angerMessage = "LET ME REST IN PEACE.";
    private Animator skeletonAnim;
    private LoreMenuVisualManager visualManager;
    private int textIdx = 0;
    private int charIdx = 0;
    private string currentText = "";
    private bool isFocused = false;
    private const float CHAR_APPEAR_INTERVAL = 0.1f;

    private void Awake()
    {
        clickAction.action.performed += ProcessInput;
        visualManager = GetComponent<LoreMenuVisualManager>();
        skeletonAnim = GameObject.FindWithTag("SkeletonJaw")?.GetComponent<Animator>();
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
        if (charIdx < currentText.Length)
        {
            StopText();
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
        currentText = "";
        visualManager.SetEyesRend(true);
        CanvasTransitionManager.OnTransitionFinished += DisplayText;
    }

    public void OnLeave()
    {
        CanvasTransitionManager.OnTransitionFinished -= DisplayText;
        StopText();
        isFocused = false;
        tmp.text = string.Empty;
    }

    private int numExtraInteractions = 0;
    private void DisplayText()
    {
        isFocused = true;
        if (currentText.Equals(angerMessage)) return;
        bool isEndMessage = textIdx >= texts.Length;
        if (isEndMessage)
        {
            currentText = endMessage;
            numExtraInteractions++;
        }
        else currentText = texts[textIdx].Trim();
        if (numExtraInteractions >= numInteractionsToAnger)
            currentText = angerMessage;
        visualManager.SetEyesRend(!currentText.Equals(endMessage));
        charIdx = 0;
        StopText();
        StartCoroutine(DisplayTextCoroutine());
    }

    private void UpdateView()
    {
        tmp.text = currentText[..charIdx] + "<alpha=#00>" + currentText[charIdx..];
    }

    private void StopText()
    {
        StopAllCoroutines();
        if(skeletonAnim) skeletonAnim.SetBool("Talking", false);
    }

    IEnumerator DisplayTextCoroutine()
    {
        if(!currentText.Equals(endMessage) && skeletonAnim) skeletonAnim.SetBool("Talking", true);
        while (charIdx <= currentText.Length)
        {
            yield return new WaitForSeconds(CHAR_APPEAR_INTERVAL * textAppearSpeed);
            UpdateView();
            charIdx++;
        }
        StopText();
    }
}