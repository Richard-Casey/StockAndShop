using UnityEngine;
using UnityEngine.UI;

public class DynamicContentSizeForTwoColumns : MonoBehaviour
{
    [Header("Layout Settings")]
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform contentArea;

    public void UpdateContentSize(int itemCount)
    {
        // Calculate the number of rows (2 items per row)
        int numberOfRows = Mathf.CeilToInt(itemCount / 2.0f);

        // Calculate the height required for one row
        float rowHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;

        // Calculate the total height needed
        float totalHeight = rowHeight * numberOfRows;

        // Update the size of the RectTransform
        contentArea.sizeDelta = new Vector2(contentArea.sizeDelta.x, totalHeight);
    }
}
