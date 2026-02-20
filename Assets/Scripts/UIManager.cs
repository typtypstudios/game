using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image ritualProgressBar;
    [SerializeField] private Image corruptionProgressBar;
    [SerializeField] private Image manaProgressBar;

    public void SetRitualProgress(float value)
    {
        ritualProgressBar.fillAmount = value;
    }

    public void SetCorruptionProgress(float value)
    {
        corruptionProgressBar.fillAmount = value;
    }

    public void SetManaProgress(float value)
    {
        manaProgressBar.fillAmount = value;
    }
}