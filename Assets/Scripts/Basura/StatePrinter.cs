using TMPro;
using UnityEngine;

public class StatePrinter : MonoBehaviour
{
    public TMP_Text tmp;

    public void Update()
    {
        tmp.text = $"State: {GameManager.Instance.CurrentState.Value}\n" +
                   $"Ritual Seed: {GameManager.Instance.RitualSeed.Value}";
    }
}
