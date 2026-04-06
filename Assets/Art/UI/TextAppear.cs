using TMPro;
using UnityEngine;

public class TextAppear : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float duration = 1f;

    private Material materialInstance;
    private float timer = 0f;

    void Awake()
    {
  
        materialInstance = text.fontMaterial;
        text.fontMaterial = materialInstance;

        materialInstance.SetFloat("_FaceDilate", -1f);
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.SmoothStep(0, 1, timer / duration);

            float dilate = Mathf.Lerp(-1f, 0f, t);

            materialInstance.SetFloat("_FaceDilate", dilate);
        }
    }
}