using UnityEngine;
using UnityEngine.UI;

public class DynamicContentSizeForOneColumn : MonoBehaviour
{
    [Header("Layout Settings")]
    private RectTransform contentRectTransform;
    private HorizontalLayoutGroup horizontalLayoutGroup;

    private float CalculateItemWidth()
    {
        // Calculate the item width based on HorizontalLayoutGroup settings
        float spacing = horizontalLayoutGroup.spacing;
        float padding = horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right;

        // Calculate the item width
        float itemWidth = spacing + padding;

        return itemWidth;
    }

    private void Start()
    {
        contentRectTransform = GetComponent<RectTransform>();
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

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
