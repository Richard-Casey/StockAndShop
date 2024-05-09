using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class DynamicContentSizeForOneColumn : MonoBehaviour
{
    /// <summary>
    /// The content rect transform
    /// </summary>
    [Header("Layout Settings")]
    private RectTransform contentRectTransform;
    /// <summary>
    /// The horizontal layout group
    /// </summary>
    private HorizontalLayoutGroup horizontalLayoutGroup;

    /// <summary>
    /// Calculates the width of the item.
    /// </summary>
    /// <returns></returns>
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
    /// Starts this instance.
    /// </summary>
    private void Start()
    {
        contentRectTransform = GetComponent<RectTransform>();
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

    /// <summary>
    /// Updates the size of the content.
    /// </summary>
    /// <param name="itemCount">The item count.</param>
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
