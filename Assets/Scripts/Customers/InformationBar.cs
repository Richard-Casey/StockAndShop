using TMPro; // Make sure to include this for TextMeshPro
using UnityEngine;
using System.Collections;

public class InformationBar : MonoBehaviour
{
    public static InformationBar Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField] private CanvasGroup canvasGroup; // Make sure to assign this
    [SerializeField] private RectTransform rectTransform; // Make sure to assign this

    public float animationDuration = 0.5f; // Duration for the rise and lower animations
    public float visibleDuration = 3f; // How long the bar stays fully visible before hiding

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // Initially set the information bar to be invisible and not interactable
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void DisplayMessage(string message, float duration = 3f)
    {
        StopAllCoroutines(); // Stop any previous animations
        StartCoroutine(ShowMessageWithAnimation(message, duration));
    }

    private IEnumerator ShowMessageWithAnimation(string message, float duration = 3f)
    {
        informationText.text = message;
        yield return AnimateBar(true); // Move the information bar into view and fade in
        yield return new WaitForSeconds(duration);
        yield return AnimateBar(false); // Move the information bar out of view and fade out
    }

    private IEnumerator AnimateBar(bool show)
    {
        float startY = show ? rectTransform.anchoredPosition.y : 0;
        float endY = show ? 0 : -rectTransform.sizeDelta.y; // Assuming moving up to 0, and down to negative height
        float startAlpha = show ? 0 : 1;
        float endAlpha = show ? 1 : 0;

        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            // Use Time.unscaledDeltaTime instead of Time.deltaTime
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
}
