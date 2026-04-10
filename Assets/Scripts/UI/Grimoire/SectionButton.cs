using System.Collections;
using TypTyp.Cults;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WritableButton))]
public class SectionButton : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.5f;
    [SerializeField] private Image cultImage;
    private Image image;
    private WritableButton button;
    private float startingFill;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<WritableButton>();
        startingFill = image.fillAmount;
    }

    public void Configurate(string name, int cultId)
    {
        button.OverrideText(name);
        if (cultId == -1) cultImage.enabled = false;
        else cultImage.sprite = CultRegister.Instance.GetById(cultId).Image;
    }

    public void Deploy()
    {
        StopAllCoroutines();
        StartCoroutine(InterpolateFill(1));
        button.CompletelyBlock(true);
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(InterpolateFill(startingFill));
        button.CompletelyBlock(false);
    }

    IEnumerator InterpolateFill(float newFill)
    {
        float speed = Mathf.Abs(1 - startingFill) / transitionTime;
        while(image.fillAmount != newFill)
        {
            image.fillAmount = Mathf.MoveTowards(image.fillAmount, newFill, speed * Time.deltaTime);
            yield return null;
        }
    }
}
