using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GrimoireContentManager))]
public class GrimoireContentGenerator : MonoBehaviour
{
    [SerializeField] private GameObject displayerPrefab;
    [SerializeField] private int numDisplayers = 9;
    [SerializeField] private GrimoireSection[] sections;
    private GrimoireContentManager contentManager;
    private GrimoireNavigationController navController;

    private void Awake()
    {
        contentManager = GetComponent<GrimoireContentManager>();
        navController = GetComponentInParent<GrimoireNavigationController>();
        for (int i = 0; i < numDisplayers; i++)
            Instantiate(displayerPrefab, this.transform);
        foreach (var section in sections) GenerateSection(section);
    }

    private void GenerateSection(GrimoireSection section)
    {
        if (section == null)
        {
            return;
        }

        navController.AddSection(contentManager.SectionStartPages.Count, section.sectionName);
        ADefinition[] definitions;
        if (section.cardRegister)
        {
            definitions = section.cardRegister.RegisteredItems
                .Where(c => c != null)
                .OrderBy(c => c.Name)
                .ToArray();
        }
        else if (section.effectRegister)
        {
            definitions = section.effectRegister.RegisteredItems
                .Where(c => c != null)
                .OrderBy(c => c.name)
                .Cast<ADefinition>()
                .ToArray();
        }
        else
        {
            Debug.LogWarning($"[GrimoireContentGenerator] Section '{section.sectionName}' has no register assigned.");
            definitions = System.Array.Empty<ADefinition>();
        }

        FillSection(definitions);
    }

    private void FillSection(ADefinition[] definition)
    {
        contentManager.SectionStartPages.Add(contentManager.Pages.Count);
        for (int i = 0; i < definition.Length; i += numDisplayers)
        {
            GrimoirePage currentPage = new(contentManager.Pages.Count,
                contentManager.SectionStartPages.Count - 1);
            for (int j = 0; j < numDisplayers; j++)
            {
                int idx = i + j;
                if (idx >= definition.Length) break;
                currentPage.definitions.Add(definition[idx]);
            }
            contentManager.Pages.Add(currentPage);
        }
    }
}
