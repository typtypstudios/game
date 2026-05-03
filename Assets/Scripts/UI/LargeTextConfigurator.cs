using UnityEngine;
using TMPro;
using TypTyp;

[RequireComponent(typeof(TMP_Text))]
public class LargeTextConfigurator : MonoBehaviour
{
    void Awake()
    {
        TryGetComponent(out TMP_Text tmp);
        if (Settings.Instance.LargeText)
        {
            tmp.enableAutoSizing = true;
            tmp.fontSizeMax = 10000;
        }
    }
}
