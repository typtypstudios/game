using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class EclipseUpdater : MonoBehaviour
{
    private TMP_Text tmp;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }
}
