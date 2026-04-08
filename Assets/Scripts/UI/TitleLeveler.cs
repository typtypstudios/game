using TypTyp.TextSystem.Typable;
using UnityEngine;

public class TitleLeveler : MonoBehaviour
{
    [SerializeField] private int numInteractionsRequired = 5;
    TypableController controller;
    private void Awake()
    {
        controller = GetComponent<TypableController>();
        controller.OnComplete += OnClick;
    }

    private void OnDestroy()
    {
        controller.OnComplete -= OnClick;
    }

    public void OnClick()
    {
        if(--numInteractionsRequired == 0)
        {
            SaveState state = SaveManager.Instance.GetState();
            foreach (var data in state.slot.cultData)
            {
                data.level = 10;
            }
            SaveManager.Instance.Save();
            SaveManager.Instance.Load();
            Debug.Log("Nivel máximo adquirido");
        }
    }
}
