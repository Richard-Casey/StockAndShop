using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartAtTop : MonoBehaviour
{
    [Header("Scroll Settings")]
    public ScrollRect scrollRect;

    void OnEnable()
    {
        StartCoroutine(ResetScrollPosition());
    }

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
