using UnityEngine;
using UnityEngine.UI;

public class SyncPositionWithButton : MonoBehaviour
{
    public RectTransform openShopButton;
    private RectTransform gameTimeRect;

    private void Start()
    {
        if (openShopButton == null)
        {
            Debug.LogError("OpenShop button reference is missing!");
            return;
        }

        gameTimeRect = GetComponent<RectTransform>();
        if (gameTimeRect == null)
        {
            Debug.LogError("GameTime object is missing RectTransform component!");
            return;
        }

        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (openShopButton != null && gameTimeRect != null)
        {
            gameTimeRect.position = openShopButton.position;
            gameTimeRect.sizeDelta = openShopButton.sizeDelta;
        }
    }
}
