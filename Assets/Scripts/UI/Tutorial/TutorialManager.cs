using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour, INavigationCtxReceiver, INavigationLeaveReceiver
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private WritableButton prevButton;
    [SerializeField] private WritableButton nextButton;
    private readonly List<GameObject> content = new();
    private int currentIdx = 0;
    private TurnPageEffect turnPageEffect;
    private bool isActive;

    void Start()
    {
        if (!TryGetComponent(out turnPageEffect))
            Debug.LogError("Error: no hay componente TurnPageEffect asociado");
        turnPageEffect.OnBlankPage += UpdateContent;
        for(int i = 0; i < contentParent.childCount; i++)
        {
            GameObject children = contentParent.GetChild(i).gameObject;
            content.Add(children);
            children.SetActive(i == 0);
        }
        turnPageEffect.InitializePages(content.Select(c => c.transform).ToArray());
        UpdateButtons();
    }

    private void OnDestroy()
    {
        turnPageEffect.OnBlankPage -= UpdateContent;
    }

    public void ReceiveContext(Screens previousScreen, bool isGoingBack)
    {
        isActive = true;
    }

    public void OnLeave()
    {
        isActive = false;
    }

    public void TurnPage(int dir)
    {
        currentIdx = Mathf.Clamp(currentIdx + dir, 0, content.Count - 1);
        turnPageEffect.TurnPage();
        UpdateButtons();
        if (isActive && AudioManager.Instance != null)
            AudioManager.Instance.PlayUI(UISound.FlipPage);
    }

    private void UpdateContent()
    {
        for(int i = 0; i < content.Count; i++)
            content[i].SetActive(i == currentIdx);
    }

    private void UpdateButtons()
    {
        prevButton.CompletelyBlock(currentIdx == 0);
        nextButton.CompletelyBlock(currentIdx == content.Count - 1);
    }
}
