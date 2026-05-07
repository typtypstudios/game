using System.Collections;
using System.Linq;
using UnityEngine;

public class MenuCultistCard : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    [Min(0)][SerializeField] private float changeTime = 5f;
    [SerializeField] private Transform back;
    [SerializeField] private bool glows = true;
    private CardVisualPresenter cardPresenter;
    private Transform mainCam;
    private CardDefinition[] cards;
    private int currentCardIdx = 0;
    private static Quaternion globalRotation = Quaternion.identity;

    private void Awake()
    {
        RuntimeVariables.Instance.OnUpdated += Init;
        if (RuntimeVariables.Instance.IsLoaded) Init();
    }

    private void OnDestroy()
    {
        if(RuntimeVariables.Instance) RuntimeVariables.Instance.OnUpdated -= Init;
    }

    private void Init()
    {
        if (cards != null) return;
        mainCam = Camera.main.transform;
        cardPresenter = GetComponentInChildren<CardVisualPresenter>();
        transform.rotation = globalRotation;
        cards = RuntimeVariables.Instance.CurrentCult.GetCards().
            OrderBy(_ => Random.value).ToArray();
        BindNextCard();
        StartCoroutine(ChangeContentCoroutine());
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        if (IsFacingCam()) back.SetAsLastSibling();
        else back.SetAsFirstSibling();
        globalRotation = transform.rotation;
    }

    private bool IsFacingCam()
    {
        Vector3 camToCard = back.position - mainCam.position;
        camToCard.y = 0;
        return Vector3.Dot(back.forward, Vector3.Normalize(camToCard)) > 0;
    }

    private void EnsureGlow()
    {
        UICardView cardView = cardPresenter.GetComponent<UICardView>();
        cardView.Details.color = cardView.Border.color;
        foreach (var emi in GetComponentsInChildren<EmissiveImageConfigurator>())
            emi.ToggleEmission(true, true);
    }

    private void BindNextCard()
    {
        var cardDefinition = cards[currentCardIdx++];
        int resolvedManaCost = Mathf.Max(0, cardDefinition.ManaCost);
        cardPresenter.SetCard(cardDefinition, resolvedManaCost, resolvedManaCost);
        if (currentCardIdx >= cards.Length) currentCardIdx = 0;
        //if(glows) EnsureGlow();
    }

    IEnumerator ChangeContentCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeTime);
            while (!IsFacingCam()) yield return null;
            BindNextCard();
        }
    }
}