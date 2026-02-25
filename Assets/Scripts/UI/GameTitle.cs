using UnityEngine;
using TMPro;

public class GameTitle : MonoBehaviour
{
    [Range(0, 1)][SerializeField] private float minColor;
    private TextMeshProUGUI title;
    private WritableButton button;

    private void Awake()
    {
        button = GetComponent<WritableButton>();
        title = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnTitledTyped()
    {
        title.color = button.FillColor;
        float r = Random.Range(minColor, 1);
        float g = Random.Range(minColor, 1);
        float b = Random.Range(minColor, 1);
        button.FillColor = new Color(r, g, b);
    }
}
