using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Animator anim;
    public Sprite Sprite 
    { 
        get { return image.sprite; } 
        set { image.sprite = value; } 
    }

    public void Destroy()
    {
        //Aquí se desplegaría la animación de que el estado se va. De momento se destruye directamente
        Destroy(this.gameObject);
    }
}
