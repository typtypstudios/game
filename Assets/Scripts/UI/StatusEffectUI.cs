using UnityEngine.UI;
using UnityEngine;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Animator anim;
    [SerializeField] private ScratchAnimation scratch;

    public Sprite Sprite 
    { 
        get { return image.sprite; } 
        set { image.sprite = value; } 
    }

    public void Destroy() => anim.SetTrigger("Destroy");

    public void OnDestroyAnimEnded() => Destroy(this.gameObject);
}
