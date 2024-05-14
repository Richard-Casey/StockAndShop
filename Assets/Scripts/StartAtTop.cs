using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Ensures that the scroll view starts at the top when the GameObject is enabled.
/// </summary>
public class StartAtTop : MonoBehaviour
{
    [Header("Scroll Settings")]

    /// <summary>
    /// The ScrollRect component to be reset to the top position.
    /// </summary>
    public ScrollRect scrollRect;

    /// <summary>
    /// Unity's OnEnable method, called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(ResetScrollPosition());
    }

    /// <summary>
    /// Coroutine to reset the scroll position to the top after UI layout is done.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
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
