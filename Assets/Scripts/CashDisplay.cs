using UnityEngine;
using TMPro;

public class CashDisplay : MonoBehaviour
{
    [Header("UI Components")]
    private TextMeshProUGUI cashText;

    [Header("Cash Settings")]
    public float cashOnHand = 50.0f;

    private void Start()
    {
        cashText = GetComponent<TextMeshProUGUI>();
        UpdateCashDisplay(); // Initialize with zero cash
    }

    public void SetCash(float amount)
    {
        cashOnHand = amount;
        UpdateCashDisplay();
        InformationBar.Instance.DisplayMessage($"Cash updated: £{cashOnHand:F2}");
    }

    public void UpdateCashDisplay()
    {
        if (cashText != null) // Check if cashText is not null
        {
            cashText.text = "Cash: £" + cashOnHand.ToString("F2");
        }
    }
}
