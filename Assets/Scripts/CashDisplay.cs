using UnityEngine;
using TMPro;

/// <summary>
/// Displays the player's cash on the UI.
/// </summary>
public class CashDisplay : MonoBehaviour
{
    /// <summary>
    /// The TextMeshPro component to display the cash amount.
    /// </summary>
    [Header("UI Components")]
    private TextMeshProUGUI cashText;

    /// <summary>
    /// The player's current cash on hand.
    /// </summary>
    [Header("Cash Settings")]
    public float cashOnHand = 50.0f;

    /// <summary>
    /// Initializes the cash display.
    /// </summary>
    private void Start()
    {
        cashText = GetComponent<TextMeshProUGUI>();
        UpdateCashDisplay(); // Initialize with zero cash
    }

    /// <summary>
    /// Sets the player's cash amount.
    /// </summary>
    /// <param name="amount">The new cash amount.</param>
    public void SetCash(float amount)
    {
        cashOnHand = amount;
        UpdateCashDisplay();
        InformationBar.Instance.DisplayMessage($"Cash updated: £{cashOnHand:F2}");
    }

    /// <summary>
    /// Updates the cash display with the current cash amount.
    /// </summary>
    public void UpdateCashDisplay()
    {
        if (cashText != null) // Check if cashText is not null
        {
            cashText.text = "Cash: £" + cashOnHand.ToString("F2");
        }
    }
}
