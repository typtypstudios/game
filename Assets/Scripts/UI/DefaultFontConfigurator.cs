using TMPro;
using TypTyp;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DefaultFontConfigurator : MonoBehaviour
{
    private TMP_Text textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        ResetFont();
    }

    public void ResetFont()
    {
        textMesh.font = Settings.Instance.DefaultFont;
    }
}
