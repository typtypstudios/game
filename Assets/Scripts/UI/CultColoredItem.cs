using TMPro;
using TypTyp.Cults;
using UnityEngine;
using UnityEngine.UI;

public class CultColoredItem : MonoBehaviour
{
    private Image image;
    private Renderer rend;
    private TMP_Text tmp;
    private ItemType type;
    private bool fixedColor = false;

    void Awake()
    {
        if (TryGetComponent(out image)) type = ItemType.Image;
        else if (TryGetComponent(out rend)) type = ItemType.Renderer;
        else if (TryGetComponent(out tmp)) type = ItemType.Text;
        SaveManager.Instance.OnAfterLoad += OnCurrentCultUpdated;
        if (RuntimeVariables.Instance.IsLoaded) OnCurrentCultUpdated(null);
    }

    private void OnDestroy() => SaveManager.Instance.OnAfterLoad -= OnCurrentCultUpdated;

    private void OnCurrentCultUpdated(SaveState _)
    {
        if (!fixedColor) UpdateColor(RuntimeVariables.Instance.CurrentCult.Color);
    }

    public void FixCult(int cultId)
    {
        fixedColor = true;
        UpdateColor(CultRegister.Instance.GetById(cultId).Color);
    }

    private void UpdateColor(Color color)
    {
        switch (type)
        {
            case ItemType.Image:
                image.color = color;
                break;
            case ItemType.Renderer:
                rend.material.color = color;
                break;
            case ItemType.Text:
                tmp.color = color;
                break;
        }
    }

    private enum ItemType { Image, Renderer, Text };
}
