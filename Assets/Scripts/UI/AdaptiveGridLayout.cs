using UnityEngine;
using System.Linq;

public class AdaptiveGridLayout : MonoBehaviour
{
    [Range(0, 0.4f)][SerializeField] private float horizontalPaddingPercentaje = 0.1f;
    [Range(0, 0.4f)][SerializeField] private float verticalPaddingPercentaje = 0.1f;
    [Range(0, 1)][SerializeField] private float spacingPercentaje = 0.02f;
    [Min(1)][SerializeField] private int numColumns = 2;
    [SerializeField] private float cellRatio = 1.34f;
    private RectTransform rectTransform;
    private RectTransform[] children;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        children = GetComponentsInChildren<RectTransform>().Where(t => t != rectTransform).ToArray();
        foreach(var rt in children)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
        }
    }

    private void LateUpdate()
    {
        int numRows = (int)Mathf.Ceil((float)children.Length / numColumns);
        float horizontalPadding = rectTransform.rect.width * horizontalPaddingPercentaje;
        float verticalPadding = rectTransform.rect.height * verticalPaddingPercentaje;
        float horizontalSpacing = spacingPercentaje * rectTransform.rect.width;
        float verticalSpacing = spacingPercentaje * rectTransform.rect.height;
        float availableVertSpace = rectTransform.rect.height - 2 * verticalPadding - 
            (numRows - 1) * verticalSpacing;
        float availableHorSpace = rectTransform.rect.width - 2 * horizontalPadding -
            (numColumns - 1) * horizontalSpacing;

        float targetHeight = availableVertSpace / numRows;
        float targetWidth = availableHorSpace / numColumns;
        float targetRatio = targetHeight / targetWidth;
        if (targetRatio > cellRatio) targetHeight = cellRatio * targetWidth;
        else targetWidth = targetHeight / cellRatio;

        horizontalPadding = (rectTransform.rect.width - (targetWidth * numColumns + 
            (numColumns - 1) * horizontalSpacing)) / 2;
        verticalPadding = (rectTransform.rect.height - (targetHeight * numRows + (numRows - 1) * 
            verticalSpacing)) / 2;

        float startXPos = horizontalPadding + targetWidth / 2;
        float startYPos = - (verticalPadding + targetHeight / 2);
        for (int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numColumns; j++)
            {
                float offsetX = startXPos + (horizontalSpacing + targetWidth) * j;
                float offsetY = startYPos - (verticalSpacing + targetHeight) * i;
                int idx = i * numColumns + j;
                if (idx >= children.Length) return;
                children[idx].anchoredPosition = new Vector2(offsetX, offsetY);
                children[idx].sizeDelta = new Vector2(targetWidth, targetHeight);
            }
        }
    }
}
