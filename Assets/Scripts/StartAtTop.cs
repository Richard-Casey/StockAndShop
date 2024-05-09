using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class StartAtTop : MonoBehaviour
{
    /// <summary>
    /// The scroll rect
    /// </summary>
    [Header("Scroll Settings")]
    public ScrollRect scrollRect;

    /// <summary>
    /// Called when [enable].
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(ResetScrollPosition());
    }

    /// <summary>
    /// Resets the scroll position.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetScrollPosition()
    {
        // Wait for the end of the frame to ensure all UI layout is done
        yield return new WaitForEndOfFrame();

        // Check if scrollRect is assigned
        if (scrollRect != null)
        {
            // Set the scrollbar to the top position
            scrollRect.verticalNormalizedPosition = 1.0f;
        }
    }
}
