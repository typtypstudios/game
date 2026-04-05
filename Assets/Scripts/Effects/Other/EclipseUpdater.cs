using TMPro;
using UnityEngine;
using TypTyp.TextSystem.Typable;

[RequireComponent(typeof(TMP_Text))]
public class EclipseUpdater : MonoBehaviour
{
    private TMP_Text tmp;
    private TypableController typableController;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        typableController = GetComponent<TypableController>();
        if (!typableController)
        {
            this.enabled = false;
            tmp.fontMaterial.SetVector("_VisibilityCenter", new(1000, 1000, 1000));
        }
    }

    private void OnEnable()
    {
        if (typableController == null) return;
        typableController.OnChanged += HandleChanged;
        typableController.OnComplete += HandleChanged;
        UpdateVisibility();
    }

    private void OnDisable()
    {
        if (typableController == null) return;
        typableController.OnChanged -= HandleChanged;
        typableController.OnComplete -= HandleChanged;
    }

    private void HandleChanged()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (tmp == null) return;
        int idx = typableController.Idx;
        tmp.ForceMeshUpdate();
        int count = tmp.textInfo.characterCount;
        if (count == 0) return;
        if (idx < 0) idx = 0;
        if (idx >= count) idx = count - 1;
        var charInfo = tmp.textInfo.characterInfo[idx];
        Vector3 localPos = (charInfo.bottomLeft + charInfo.topRight) / 2f;
        tmp.fontMaterial.SetVector("_VisibilityCenter", transform.TransformPoint(localPos));
    }
}
