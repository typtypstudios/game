using TypTyp.Cults;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardBack : MonoBehaviour
{
    [SerializeField] private bool searchInParentPlayer;
    private Image image;
    private Player player;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (searchInParentPlayer) player = GetComponentInParent<Player>();
    }

    void OnEnable()
    {
        if (searchInParentPlayer) player.OnPlayerConfigurated += BindPlayerCult;
        else
        {
            if (RuntimeVariables.Instance.IsLoaded) BindCurrentCult();
            else RuntimeVariables.Instance.OnUpdated += BindCurrentCult;
        }
    }

    private void OnDisable()
    {
        if (searchInParentPlayer && player != null) player.OnPlayerConfigurated -= BindPlayerCult;
        else if (RuntimeVariables.Instance) RuntimeVariables.Instance.OnUpdated -= BindCurrentCult;
    }

    private void BindCurrentCult()
    {
        image.sprite = CultRegister.Instance.GetById(RuntimeVariables.Instance.CurrentCultID).CardBack;
    }

    private void BindPlayerCult()
    {
        image.sprite = CultRegister.Instance.GetById(player.CultID).CardBack;
    }
}
