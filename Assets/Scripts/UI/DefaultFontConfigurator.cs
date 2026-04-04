using TMPro;
using TypTyp;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DefaultFontConfigurator : MonoBehaviour
{
    [SerializeField] private bool ignoreRestriction = false;
    private TMP_Text textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        ResetFont();
    }

    public void ResetFont()
    {
        bool isRitual = GetComponentInParent<Player>() != null;
        if (Settings.Instance.OnlyApplyDefaultFontOnRitual && !isRitual && !ignoreRestriction) return;
        if(textMesh != null) textMesh.font = Settings.Instance.DefaultFont;
    }
}
