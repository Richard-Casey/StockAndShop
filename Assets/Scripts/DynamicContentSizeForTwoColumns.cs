using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adjusts the size of a UI element to fit two columns of content dynamically.
/// </summary>
public class DynamicContentSizeForTwoColumns : MonoBehaviour
{
    /// <summary>
    /// The RectTransform component of the content area.
    /// </summary>
    public RectTransform contentArea;

    /// <summary>
    /// The GridLayoutGroup component for the layout settings.
    /// </summary>
    [Header("Layout Settings")]
    public GridLayoutGroup gridLayoutGroup;

    /// <summary>
    /// Updates the size of the content area to fit the specified number of items.
    /// </summary>
    /// <param name="itemCount">The number of items in the content area.</param>
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
