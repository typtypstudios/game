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
        if (searchInParentPlayer) player.OnCultConfigurated += BindImage;
        else
        {
            if (RuntimeVariables.Instance.IsLoaded) BindImage(RuntimeVariables.Instance.CurrentCultID);
            else RuntimeVariables.Instance.OnUpdated += BindCurrentCult;
        }
    }

    private void OnDisable()
    {
        if (searchInParentPlayer && player != null) player.OnCultConfigurated -= BindImage;
        else if (RuntimeVariables.Instance) RuntimeVariables.Instance.OnUpdated -= BindCurrentCult;
    }

    private void BindCurrentCult()
    {
        BindImage(RuntimeVariables.Instance.CurrentCultID);
    }

    private void BindImage(int cultId)
    {
        image.sprite = CultRegister.Instance.GetById(cultId).CardBack;
    }
}
