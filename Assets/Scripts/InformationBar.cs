using TMPro; 
using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the information bar UI element, displaying messages with animations.
/// </summary>
public class InformationBar : MonoBehaviour
{
    /// <summary>
    /// The canvas group for controlling visibility and interaction.
    /// </summary>
    [SerializeField] private CanvasGroup canvasGroup; 

    /// <summary>
    /// The text component for displaying information.
    /// </summary>
    [SerializeField] private TextMeshProUGUI informationText;

    /// <summary>
    /// The RectTransform component of the information bar.
    /// </summary>
    [SerializeField] private RectTransform rectTransform;

    /// <summary>
    /// Duration for the rise and lower animations.
    /// </summary>
    public float animationDuration = 0.5f; // Duration for the rise and lower animations

    /// <summary>
    /// How long the bar stays fully visible before hiding.
    /// </summary>
    public float visibleDuration = 3f; // How long the bar stays fully visible before hiding

    /// <summary>
    /// Singleton instance of the InformationBar class.
    /// </summary>
    public static InformationBar Instance { get; private set; }

    /// <summary>
    /// Animates the information bar to show or hide.
    /// </summary>
    /// <param name="show">Whether to show or hide the bar.</param>
    /// <returns>An IEnumerator for the animation coroutine.</returns>
    private IEnumerator AnimateBar(bool show)
    {
        float startY = show ? rectTransform.anchoredPosition.y : 0;
        float endY = show ? 0 : -rectTransform.sizeDelta.y; // Moving up to 0, and down to negative height
        float startAlpha = show ? 0 : 1;
        float endAlpha = show ? 1 : 0;

        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float newPosY = Mathf.Lerp(startY, endY, elapsedTime / animationDuration);
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / animationDuration);

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newPosY);
            canvasGroup.alpha = newAlpha;

            yield return null;
        }

        // Ensure final position and alpha are set after the loop
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, endY);
        canvasGroup.alpha = endAlpha;

        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }

    /// <summary>
    /// Initializes the InformationBar instance and sets initial visibility.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // Initially set the information bar to be invisible and not interactable
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Shows a message with animation.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="duration">The duration for which the message is visible.</param>
    /// <returns>An IEnumerator for the animation coroutine.</returns>
    private IEnumerator ShowMessageWithAnimation(string message, float duration = 3f)
    {
        informationText.text = message;
        yield return AnimateBar(true); // Move the information bar into view and fade in
        yield return new WaitForSeconds(duration);
        yield return AnimateBar(false); // Move the information bar out of view and fade out
    }

    /// <summary>
    /// Displays a message on the information bar.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="duration">The duration for which the message is visible.</param>
    public void DisplayMessage(string message, float duration = 3f)
    {
        StopAllCoroutines(); // Stop any previous animations
        StartCoroutine(ShowMessageWithAnimation(message, duration));
    }

}
