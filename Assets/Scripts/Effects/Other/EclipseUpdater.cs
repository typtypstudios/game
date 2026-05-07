using TMPro;
using UnityEngine;
using TypTyp.TextSystem.Typable;
using TypTyp;
using Unity.VisualScripting;

[RequireComponent(typeof(TMP_Text))]
public class EclipseUpdater : MonoBehaviour
{
    private TMP_Text tmp;
    private TypableController typableController;
    private TMPTypableView view;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        typableController = GetComponent<TypableController>();
        view = GetComponent<TMPTypableView>();
        if (!typableController || !view)
        {
            this.enabled = false;
            tmp.fontMaterial.SetVector("_VisibilityCenter", new(1000, 1000, 1000));
        }
    }

    private void Update()
    {
        if (tmp == null) return;
        tmp.ForceMeshUpdate();
        int count = tmp.textInfo.characterCount;
        if (count == 0) return;
        int idx = Mathf.Clamp(typableController.Idx, 0, count - 1);
        HandleSpaces(ref idx);
        var charInfo = tmp.textInfo.characterInfo[idx];
        Vector3 localPos = (charInfo.bottomLeft + charInfo.topRight) / 2f;
        tmp.fontMaterial.SetVector("_VisibilityCenter", transform.TransformPoint(localPos));
    }
    private void HandleSpaces(ref int idx)
    {
        if (!tmp || typableController.Text == null || typableController.Text.Equals(string.Empty)) return;
        if (Settings.Instance.ShowSpaces && view.Config.isAbleToShowSpaces)
        {
            string original = typableController.Text;
            int spaceCount = original[..idx].CountIndices(' ');
            int spaceAdditionalLength = Utils.ParseUnicodeEscapes(Settings.Instance.SpaceReplacement).Length - 1;
            idx += spaceAdditionalLength * spaceCount;
        }
    }
}
