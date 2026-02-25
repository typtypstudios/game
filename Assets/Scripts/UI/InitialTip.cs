using UnityEngine;

public class InitialTip : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (PlayerPrefs.GetInt("Understood", 0) != 1)
        {
            canvasGroup.alpha = 1;
            mainMenu.SetActive(false);
        }
    }

    public void Understood()
    {
        PlayerPrefs.SetInt("Understood", 1);
        canvasGroup.alpha = 0;
        mainMenu.SetActive(true);
        canvasGroup.blocksRaycasts = false;
    }
}
