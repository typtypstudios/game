using TypTyp.Cults;
using UnityEngine;

public class CultSelection : MonoBehaviour
{
    [SerializeField] private GameObject cultButtonPrefab;
    [SerializeField] private Transform cultButtonsParent;

    void Start()
    {
        foreach(var cultInfo in RuntimeVariables.Instance.CultsInfo)
        {
            CultButton cultButton = Instantiate(cultButtonPrefab, cultButtonsParent).GetComponent<CultButton>();
            cultButton.SetCultInfo(cultInfo);
        }
    }
}
