using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private UISound sound = UISound.ButtonClick;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Play);
    }

    private void Play()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUI(sound);
    }
}