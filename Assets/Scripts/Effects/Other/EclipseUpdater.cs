using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class EclipseUpdater : MonoBehaviour
{
    private TMP_Text tmp;
    private AInputListener listener;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        listener = GetComponent<AInputListener>();
        if (!listener)
        {
            this.enabled = false;
            tmp.fontMaterial.SetVector("_VisibilityCenter", new(1000, 1000, 1000));
        }
    }

    private void Update()
    {
        //if(listener is WritableSpell) SetVisibilityAtChar(tmp.text.Length / 2);
        //else SetVisibilityAtChar(listener.Idx);
        SetVisibilityAtChar(listener.Idx);
    }

    private void SetVisibilityAtChar(int idx)
    {
        tmp.ForceMeshUpdate();
        var charInfo = tmp.textInfo.characterInfo[idx];
        Vector3 localPos = (charInfo.bottomLeft + charInfo.topRight) / 2f;
        tmp.fontMaterial.SetVector("_VisibilityCenter", transform.TransformPoint(localPos));
    }
}
