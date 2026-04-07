using System.Collections.Generic;
using System.Linq;
using TypTyp.Cults;
using UnityEngine;

[RequireComponent(typeof(GrimoireContentManager))]
public class GrimoireContentGenerator : MonoBehaviour
{
    [SerializeField] private GameObject displayerPrefab;
    [SerializeField] private int numDisplayers = 9;
    [SerializeField] private string basicSpellsLabel = "Basic Spells";
    [SerializeField] private string effectsSectionLabel = "Status Effects";
    private GrimoireContentManager contentManager;
    private GrimoireNavigationController navController;

    private void Awake()
    {
        contentManager = GetComponent<GrimoireContentManager>();
        navController = GetComponentInParent<GrimoireNavigationController>();
        for (int i = 0; i < numDisplayers; i++)
            Instantiate(displayerPrefab, this.transform);
        //Cartas base:
        GenerateSection(CardRegister.Instance.RegisteredItems.Where(c => c.Cult == null), basicSpellsLabel);
        //Cartas por culto:
        foreach(CultDefinition cult in CultRegister.Instance.RegisteredItems)
            GenerateSection(cult.GetCards().Cast<ADefinition>(), cult.Name);
        //Effectos:
        GenerateSection(StatusEffectRegister.Instance.RegisteredItems.Cast<ADefinition>(), 
            effectsSectionLabel);
    }

    private void GenerateSection(IEnumerable<ADefinition> definitions, string name)
    {
        navController.AddSection(contentManager.SectionStartPages.Count, name);
        contentManager.SectionStartPages.Add(contentManager.Pages.Count);
        FillSection(definitions.Distinct().OrderBy(d => d.Name).ToArray());
    }

    private void FillSection(ADefinition[] definitions)
    {
        for (int i = 0; i < definitions.Length; i += numDisplayers)
        {
            GrimoirePage currentPage = new(contentManager.Pages.Count,
                contentManager.SectionStartPages.Count - 1);
            for (int j = 0; j < numDisplayers; j++)
            {
                int idx = i + j;
                if (idx >= definitions.Length) break;
                currentPage.definitions.Add(definitions[idx]);
            }
            contentManager.Pages.Add(currentPage);
        }
    }
}
