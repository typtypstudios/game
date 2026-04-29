using System.Linq;
using UnityEngine;

public class GameSettingsNavigator : MonoBehaviour
{
    [SerializeField] private GameObject[] sections;
    [Tooltip("Los botones de navegación deben ir en el orden de las secciones")]
    [SerializeField] private WritableButton[] navigationButtons;
    private TurnPageEffect changeEffect;
    int currentSection = 0;

    private void Awake()
    {
        if(!TryGetComponent(out changeEffect)) 
            Debug.LogError("Error: el navegador de settings no cuenta con efecto de cambio.");
        changeEffect.OnBlankPage += HandleChange;
        changeEffect.InitializePages(sections.Select(s => s.transform).ToArray());
    }

    private void Start()
    {
        HandleChange();
        CheckButtonsState();
    }

    public void SetSection(int sectionIdx)
    {
        currentSection = sectionIdx;
        changeEffect.TurnPage();
        CheckButtonsState();
    }

    private void CheckButtonsState()
    {
        for (int i = 0; i < navigationButtons.Length; i++)
        {
            navigationButtons[i].CompletelyBlock(i == currentSection);
        }
    }

    private void HandleChange()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            sections[i].SetActive(i == currentSection);
        }
    }
}
