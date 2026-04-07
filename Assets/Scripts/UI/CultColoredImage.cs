using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CultColoredImage : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        SaveManager.Instance.OnAfterLoad += UpdateColor;
        if (RuntimeVariables.Instance.IsLoaded) UpdateColor(null);
    }

    private void UpdateColor(SaveState _)
    {
        image.color = RuntimeVariables.Instance.CurrentCult.Color;
    }
}
