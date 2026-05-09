using TypTyp;
using UnityEngine;

public class UIInkBar : UIBarGroup
{
    [Tooltip("Separación entre barras expresado en porcentaje del width de mana bar")]
    [Range(0f, 0.1f)][SerializeField] private float barSeparation = 0.01f;
    [SerializeField] private GameObject manaBarPrefab;

    private void Awake()
    {
        CreateBars();
    }

    private void CreateBars() //Se colocan las barras manualmente y no en un layout group porque va mejor
    {
        float totalWidth = GetComponent<RectTransform>().sizeDelta.x;
        float totalSeparation = (Settings.Instance.NumManaBars - 1) * barSeparation * totalWidth;
        float barWidth = (totalWidth - totalSeparation) / Settings.Instance.NumManaBars;
        float currentPos = barWidth / 2; //Posición de la primera barra
        for(int i = 0; i < Settings.Instance.NumManaBars; i++)
        {
            RectTransform bar = Instantiate(manaBarPrefab, transform).GetComponent<RectTransform>();
            bar.sizeDelta = new Vector2(barWidth, bar.sizeDelta.y);
            bar.anchoredPosition = new Vector2(currentPos, 0);
            currentPos += barWidth + barSeparation * totalWidth;
            InkOrb inkBar = bar.GetComponentInChildren<IFillableBar>() as InkOrb;
            if (i != 0) inkBar.PrevOrb = bars[i - 1];
            bars.Add(inkBar);
        }
    }
}
