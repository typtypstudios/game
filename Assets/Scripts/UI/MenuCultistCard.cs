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
        mainCam = Camera.main.transform;
        cardPresenter = GetComponentInChildren<CardVisualPresenter>();
    }

    private void OnEnable()
    {
        transform.rotation = globalRotation;
        if (RuntimeVariables.Instance.IsLoaded) InitCards();
        else RuntimeVariables.Instance.OnUpdated += InitCards;
            StartCoroutine(ChangeContentCoroutine());
    }

    private void OnDestroy()
    {
        if(RuntimeVariables.Instance) RuntimeVariables.Instance.OnUpdated -= InitCards;
    }

    private void InitCards()
    {
        cards = RuntimeVariables.Instance.CurrentCult.GetCards().
            OrderBy(_ => Random.value).ToArray();
        RuntimeVariables.Instance.OnUpdated -= InitCards;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        if (FacingCam()) back.SetAsFirstSibling();
        else back.SetAsLastSibling();
        globalRotation = transform.rotation;
    }

    private bool FacingCam()
    {
        Vector3 camToCard = transform.position - mainCam.position;
        camToCard.y = 0;
        bool isFacing = Vector3.Dot(transform.forward, Vector3.Normalize(camToCard)) < 0;
        bool isOnCamera = Vector3.Dot(mainCam.forward, camToCard) > 0;
        return isFacing && isOnCamera;
    }

    private void EnsureGlow()
    {
        UICardView cardView = cardPresenter.GetComponent<UICardView>();
        if(cardView.Details) cardView.Details.color = cardView.Border.color;
        foreach (var emi in GetComponentsInChildren<EmissiveImageConfigurator>())
            emi.ToggleEmission(true, true);
    }

    private void BindNextCard()
    {
        var cardDefinition = cards[currentCardIdx++];
        int resolvedManaCost = Mathf.Max(0, cardDefinition.ManaCost);
        cardPresenter.SetCard(cardDefinition, resolvedManaCost, resolvedManaCost);
        if (currentCardIdx >= cards.Length) currentCardIdx = 0;
        if(glows) EnsureGlow();
    }

    IEnumerator ChangeContentCoroutine()
    {
        while (cards == null) yield return null;
        yield return null;
        BindNextCard();
        while (true)
        {
            yield return new WaitForSeconds(changeTime);
            while (FacingCam()) yield return null;
            BindNextCard();
        }
    }
}