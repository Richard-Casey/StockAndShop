using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class CashDisplay : MonoBehaviour
{
    /// <summary>
    /// The cash text
    /// </summary>
    [Header("UI Components")]
    private TextMeshProUGUI cashText;

    /// <summary>
    /// The cash on hand
    /// </summary>
    [Header("Cash Settings")]
    public float cashOnHand = 50.0f;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    private void Start()
    {
        cashText = GetComponent<TextMeshProUGUI>();
        UpdateCashDisplay(); // Initialize with zero cash
    }

    /// <summary>
    /// Sets the cash.
    /// </summary>
    /// <param name="amount">The amount.</param>
    public void SetCash(float amount)
    {
        cashOnHand = amount;
        UpdateCashDisplay();
        InformationBar.Instance.DisplayMessage($"Cash updated: £{cashOnHand:F2}");
    }

    /// <summary>
    /// Updates the cash display.
    /// </summary>
    public void UpdateCashDisplay()
    {
        if (cashText != null) // Check if cashText is not null
        {
            cashText.text = "Cash: £" + cashOnHand.ToString("F2");
        }
    }
}
