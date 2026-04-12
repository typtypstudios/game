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
    [Range(-1, 1)][SerializeField] private float verticalOffset = 0;
    [Range(-1, 1)][SerializeField] private float horizontalOffset = 0;

    [Header("Cell related")]
    [Min(1)][SerializeField] private int numColumns = 2;
    [SerializeField] private float cellRatio = 1.3333333f;
    [SerializeField] private bool centerIncompleteRows = true;

    [Header("Excluded objects")]
    [SerializeField] private Transform[] excludedObjects;

    private RectTransform rectTransform;
    private readonly List<RectTransform> children = new();

    // 🔑 Nuevo: control de cambios reales en hijos
    private int lastChildCount = -1;

#if UNITY_EDITOR
    private void OnEnable()
    {
        RefreshChildren();
        lastChildCount = transform.childCount;
    }
#endif

    private void Start()
    {
        RefreshChildren();
        lastChildCount = transform.childCount;
    }

    private void LateUpdate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        //Solo refrescar si cambia el número de hijos (no el orden)
        if (transform.childCount != lastChildCount)
        {
            RefreshChildren();
            lastChildCount = transform.childCount;
        }

        children.RemoveAll(child => child == null);

        if (rectTransform == null || children.Count == 0 || numColumns <= 0)
            return;

        int numRows = (int)Mathf.Ceil((float)children.Count / numColumns);

        float horizontalPadding = rectTransform.rect.width * horizontalPaddingPercentaje;
        float verticalPadding = rectTransform.rect.height * verticalPaddingPercentaje;
        float horizontalSpacing = horizontalSpacingPercentaje * rectTransform.rect.width;
        float verticalSpacing = verticalSpacingPercentaje * rectTransform.rect.height;

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

        verticalPadding = (rectTransform.rect.height - (targetHeight * numRows +
            (numRows - 1) * verticalSpacing)) / 2;

        float startXPos = horizontalPadding + targetWidth / 2 + horizontalPadding * horizontalOffset;
        float startYPos = -(verticalPadding + targetHeight / 2) + verticalPadding * verticalOffset;

        int incompletedCount = 0;

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numColumns; j++)
            {
                float offsetX = startXPos + (horizontalSpacing + targetWidth) * j;
                float offsetY = startYPos - (verticalSpacing + targetHeight) * i;

                int idx = i * numColumns + j;

                if (idx >= children.Count)
                {
                    incompletedCount = idx % numColumns;
                    break;
                }

                RectTransform child = children[idx];
                if (child == null) continue;

                child.anchoredPosition = new Vector2(offsetX, offsetY);
                child.sizeDelta = new Vector2(targetWidth, targetHeight);
            }
        }

        if (!centerIncompleteRows || incompletedCount == 0) return;

        horizontalPadding = (rectTransform.rect.width - (targetWidth * incompletedCount +
            (incompletedCount - 1) * horizontalSpacing)) / 2;

        startXPos = horizontalPadding + targetWidth / 2;

        for (int i = 0; i < incompletedCount; i++)
        {
            float offsetX = startXPos + (horizontalSpacing + targetWidth) * i;
            int idx = incompletedCount - i;

            RectTransform child = children[^idx];
            if (child == null) continue;

            child.anchoredPosition = new Vector2(offsetX, child.anchoredPosition.y);
        }
    }

    private void RefreshChildren()
    {
        rectTransform = GetComponent<RectTransform>();
        children.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (excludedObjects.Contains(child)) continue;

            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);

            children.Add(rt);
        }
    }
}