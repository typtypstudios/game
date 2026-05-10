using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RitualPlaceholder : MonoBehaviour
{
    [SerializeField] private TMP_Text mottoText;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        RuntimeVariables.Instance.OnUpdated += OnCurrentCultUpdated;
        if (RuntimeVariables.Instance.IsLoaded) OnCurrentCultUpdated();
    }

    private void OnDestroy()
    {
        if(RuntimeVariables.Instance) RuntimeVariables.Instance.OnUpdated -= OnCurrentCultUpdated;
    }

    private void OnCurrentCultUpdated()
    {
        image.sprite = RuntimeVariables.Instance.CurrentCult.Image;
        mottoText.text = RuntimeVariables.Instance.CurrentCult.Motto;
    }
}
