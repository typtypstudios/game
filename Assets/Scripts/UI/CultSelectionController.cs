using TypTyp.Cults;
using UnityEngine;

public class CultSelectionController : MonoBehaviour
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

    public void SetCult(int cultId)
    {
        SaveState state = SaveManager.Instance.GetState();
        state.slot.cultId = cultId;
        SaveManager.Instance.Save(false);
        SaveManager.Instance.Load();
    }
}
