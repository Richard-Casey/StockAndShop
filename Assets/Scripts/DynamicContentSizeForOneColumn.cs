using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adjusts the size of a UI element to fit a single column of content dynamically.
/// </summary>
public class DynamicContentSizeForOneColumn : MonoBehaviour
{
    [Header("Layout Settings")]

    /// <summary>
    /// The RectTransform component of the content.
    /// </summary>
    private RectTransform contentRectTransform;

    /// <summary>
    /// The HorizontalLayoutGroup component of the content.
    /// </summary>
    private HorizontalLayoutGroup horizontalLayoutGroup;

    /// <summary>
    /// Calculates the width of each item based on the layout settings.
    /// </summary>
    /// <returns>The calculated width of an item.</returns>
    private float CalculateItemWidth()
    {
        // Calculate the item width based on HorizontalLayoutGroup settings
        float spacing = horizontalLayoutGroup.spacing;
        float padding = horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right;

        // Calculate the item width
        float itemWidth = spacing + padding;

        return itemWidth;
    }

    /// <summary>
    /// Initializes the components.
    /// </summary>
    private void Start()
    {
        contentRectTransform = GetComponent<RectTransform>();
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

    /// <summary>
    /// Updates the size of the content to fit the specified number of items.
    /// </summary>
    /// <param name="itemCount">The number of items in the content.</param>
    public void UpdateContentSize(int itemCount)
    {
        if (contentRectTransform != null && horizontalLayoutGroup != null)
        {
            // Calculate the width of the content based on the layout settings
            float itemWidth = CalculateItemWidth();
            float spacing = horizontalLayoutGroup.spacing;
            float padding = horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right;

            float contentWidth = itemCount * (itemWidth + spacing) + padding;

            // Update the content size
            contentRectTransform.sizeDelta = new Vector2(contentWidth, contentRectTransform.sizeDelta.y);
        }
    }
}
