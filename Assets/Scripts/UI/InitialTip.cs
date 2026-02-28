using UnityEngine;

public class InitialTip : MonoBehaviour
{
    [SerializeField] private Canvas mainMenuCanvas;
    private Canvas selfCanvas;

    private void Awake()
    {
        selfCanvas = GetComponent<Canvas>();
        if (PlayerPrefs.GetInt("Understood", 0) != 1)
        {
            selfCanvas.enabled = true;
            mainMenuCanvas.enabled = false;
        }
    }

    public void Understood()
    {
        PlayerPrefs.SetInt("Understood", 1);
        selfCanvas.enabled = false;
        mainMenuCanvas.enabled = true;
    }
}
