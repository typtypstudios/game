using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
public class AdaptiveGridLayout : MonoBehaviour
{
    [Header("Padding & spacing")]
    [Range(0, 0.4f)][SerializeField] private float horizontalPaddingPercentaje = 0.1f;
    [Range(0, 0.4f)][SerializeField] private float verticalPaddingPercentaje = 0.1f;
    [Range(0, 1)][SerializeField] private float verticalSpacingPercentaje = 0.02f;
    [Range(0, 1)][SerializeField] private float horizontalSpacingPercentaje = 0.02f;
    [Header("Cell related")]
    [Min(1)][SerializeField] private int numColumns = 2;
    [SerializeField] private float cellRatio = 1.3333333f;
    [SerializeField] private bool centerIncompleteRows = true;
    [Header("Excluded objects")]
    [SerializeField] private Transform[] excludedObjects;
    private RectTransform rectTransform;
    private readonly List<RectTransform> children = new();

#if UNITY_EDITOR
    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        children.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (excludedObjects.Contains(transform.GetChild(i))) continue;
            RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            children.Add(rt);
        }
    }
#endif

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        children.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (excludedObjects.Contains(transform.GetChild(i))) continue;
            RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            children.Add(rt);
        }
    }

    private void LateUpdate()
    {
        //Cálculo de espacio disponible para las cartas:
        int numRows = (int)Mathf.Ceil((float)children.Count / numColumns);
        float horizontalPadding = rectTransform.rect.width * horizontalPaddingPercentaje;
        float verticalPadding = rectTransform.rect.height * verticalPaddingPercentaje;
        float horizontalSpacing = horizontalSpacingPercentaje * rectTransform.rect.width;
        float verticalSpacing = verticalSpacingPercentaje * rectTransform.rect.height;
        float availableVertSpace = rectTransform.rect.height - 2 * verticalPadding - 
            (numRows - 1) * verticalSpacing;
        float availableHorSpace = rectTransform.rect.width - 2 * horizontalPadding -
            (numColumns - 1) * horizontalSpacing;
        //Cálculo tamaño de las cartas:
        float targetHeight = availableVertSpace / numRows;
        float targetWidth = availableHorSpace / numColumns;
        float targetRatio = targetHeight / targetWidth;
        //Conservación del aspect ratio:
        if (targetRatio > cellRatio) targetHeight = cellRatio * targetWidth;
        else targetWidth = targetHeight / cellRatio;
        //Centrado de las cartas una vez se ha conservado el aspect ratio:
        horizontalPadding = (rectTransform.rect.width - (targetWidth * numColumns + 
            (numColumns - 1) * horizontalSpacing)) / 2;
        verticalPadding = (rectTransform.rect.height - (targetHeight * numRows + (numRows - 1) * 
            verticalSpacing)) / 2;
        //Colocación de las cartas:
        float startXPos = horizontalPadding + targetWidth / 2;
        float startYPos = - (verticalPadding + targetHeight / 2);
        int incompletedCount = 0; //Número de cartas de una fila incompleta
        for (int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numColumns; j++)
            {
                float offsetX = startXPos + (horizontalSpacing + targetWidth) * j;
                float offsetY = startYPos - (verticalSpacing + targetHeight) * i;
                int idx = i * numColumns + j;
                if (idx >= children.Count)
                {
                    incompletedCount = idx % numColumns;
                    break;
                }
                children[idx].anchoredPosition = new Vector2(offsetX, offsetY);
                children[idx].sizeDelta = new Vector2(targetWidth, targetHeight);
            }
        }
        //Centrado de filas incompletas:
        if (!centerIncompleteRows || incompletedCount == 0) return;
        horizontalPadding = (rectTransform.rect.width - (targetWidth * incompletedCount +
            (incompletedCount - 1) * horizontalSpacing)) / 2;
        startXPos = horizontalPadding + targetWidth / 2;
        for (int i = 0; i < incompletedCount; i++)
        {
            float offsetX = startXPos + (horizontalSpacing + targetWidth) * i;
            int idx = incompletedCount - i;
            children[^idx].anchoredPosition = new Vector2(offsetX, children[^idx].anchoredPosition.y);
        }
    }
}