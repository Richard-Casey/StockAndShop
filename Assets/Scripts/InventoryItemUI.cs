using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// </summary>
public class InventoryItemUI : MonoBehaviour
{
    /// <summary>
    /// The item image component
    /// </summary>
    private Image itemImageComponent;
    /// <summary>
    /// The original cost
    /// </summary>
    private float originalCost;

    // Private fields and methods
    /// <summary>
    /// The selling price
    /// </summary>
    private float sellingPrice = 0.0f;
    /// <summary>
    /// The demand bar
    /// </summary>
    [Header("UI Components")]
    public Image demandBar;

    /// <summary>
    /// The demand bar material
    /// </summary>
    [Header("Misc")]
    public MaterialPropertyBlock demandBarMaterial;
    /// <summary>
    /// The inventory manager
    /// </summary>
    public InventoryManager inventoryManager;
    /// <summary>
    /// The is selected for selling
    /// </summary>
    public bool isSelectedForSelling = false;
    /// <summary>
    /// The item cost
    /// </summary>
    public float itemCost;

    /// <summary>
    /// The item name
    /// </summary>
    [Header("Item Settings")]
    public string itemName;
    /// <summary>
    /// The minus button
    /// </summary>
    public Button minusButton;
    /// <summary>
    /// The plus button
    /// </summary>
    public Button plusButton;
    /// <summary>
    /// The price input field
    /// </summary>
    public TMP_InputField priceInputField;
    /// <summary>
    /// The quantity
    /// </summary>
    public int quantity = 0;
    /// <summary>
    /// The sell button
    /// </summary>
    public Button sellButton;
    /// <summary>
    /// The sell quantity
    /// </summary>
    public int sellQuantity = 0;
    /// <summary>
    /// The sell quantity text
    /// </summary>
    public TextMeshProUGUI sellQuantityText;

    /// <summary>
    /// The wholesale manager
    /// </summary>
    [Header("Manager References")]
    public WholesaleManager wholesaleManager;


    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        // Find the GameManager GameObject in the scene
        GameObject gameManagerObject = GameObject.Find("GameManager");

        if (gameManagerObject != null)
        {
            // Get the WholesaleManager component from the GameManager GameObject
            wholesaleManager = gameManagerObject.GetComponent<WholesaleManager>();
        }
        else
        {
            Debug.LogError("GameManager GameObject not found in the scene.");
        }
    }

    /// <summary>
    /// Calculates the original cost.
    /// </summary>
    /// <returns></returns>
    private float CalculateOriginalCost()
    {
        // Check if the wholesale manager reference is assigned
        if (wholesaleManager != null)
        {
            // Find the item in the wholesaleItems list based on its name
            InventoryItem item = wholesaleManager.wholesaleItems.Find(i => i.cost == itemCost);

            // If the item is found, return its cost as the original cost
            if (item != null)
            {
                return item.cost;
            }
        }

        return 0.0f; //adjust this value as needed
    }



    /// <summary>
    /// Gets the item quantity in inventory.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    private int GetItemQuantityInInventory(string itemName)
    {

        foreach (InventoryItem item in inventoryManager.inventoryItems)
        {
            if (item.itemName == itemName)
            {
                return item.quantity;
            }
        }

        // If the item is not found in the inventory, return 0 or any appropriate default value.
        return 0;
    }

    /// <summary>
    /// Gets the maximum quantity in inventory.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    private int GetMaxQuantityInInventory(string itemName)
    {
        int maxQuantity = 0;
        foreach (InventoryItem item in inventoryManager.inventoryItems)
        {
            if (item.itemName == itemName)
            {
                maxQuantity = item.quantity;
                break; // We found the item, no need to continue iterating
            }
        }
        return maxQuantity;
    }

    /// <summary>
    /// Called when [sell button clicked].
    /// </summary>
    void OnSellButtonClicked()
    {
        Debug.Log("[InventoryItemUI] Sell button clicked for " + itemName);
        Debug.Log($"[OnSellButtonClicked] Before Selling: Item = {itemName}, Quantity in Inventory = {quantity}, Sell Quantity = {sellQuantity}");
        isSelectedForSelling = !isSelectedForSelling;

        // Check if there's something to sell
        if (sellQuantity > 0)
        {
            // Store the selling price from the PriceInputField
            if (float.TryParse(priceInputField.text, out float enteredPrice))
            {
                sellingPrice = enteredPrice;
            }

            // Find the ShelfManager in the scene
            ShelfManager shelfManager = FindObjectOfType<ShelfManager>();

            // Check if the ShelfManager was found
            if (shelfManager != null)
            {
                // Create a new InventoryItem to represent the item being sold
                InventoryItem soldItem = new InventoryItem
                {
                    itemName = itemName,
                    cost = itemCost,
                    quantity = sellQuantity, // Use sellQuantity here
                    itemImage = itemImageComponent.sprite,
                    demand = CalculateOriginalCost(),
                    baseDemand = CalculateOriginalCost(),
                    sellingPrice = sellingPrice
                };

                // Add the sold item to the Shelf Items list in the ShelfManager
                shelfManager.AddToShelfItems(soldItem);

                InformationBar.Instance.DisplayMessage($"{sellQuantity}x {itemName} sent to shop floor for £{sellingPrice:F2} each");

            }
            else
            {
                Debug.LogError("ShelfManager not found in the scene.");
            }

            // Deduct the sold quantity from the inventory quantity and reset sellQuantity
            quantity -= sellQuantity;

            // After deducting the sold quantity
            if (quantity <= 0)
            {
                inventoryManager.RemoveItem(itemName);
            }

            if (inventoryManager != null)
            {
                inventoryManager.UpdateInventoryItemQuantity(itemName, quantity);
            }

            sellQuantity = 0;

            // Update the UI to reflect the changes
            UpdateUI();
        }
        Debug.Log($"[OnSellButtonClicked] After Selling: Item = {itemName}, Quantity in Inventory = {quantity}, Sell Quantity Reset to = {sellQuantity}");
    }

    /// <summary>
    /// Sets the color of the demand bar.
    /// </summary>
    /// <param name="color">The color.</param>
    private void SetDemandBarColor(Color color)
    {
        if (demandBar != null)
        {
            demandBar.color = color;
        }
        else
        {
            Debug.LogError("Demand bar is not assigned or is not an Image component.");
        }
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    private void Start()
    {
        Debug.Log("[InventoryItemUI] Start called for item: " + itemName);

        originalCost = CalculateOriginalCost();
        Debug.Log($"[InventoryItemUI] Original cost for {itemName} set at {originalCost}");

        plusButton.onClick.AddListener(() => IncrementQuantity());
        minusButton.onClick.AddListener(DecrementQuantity);
        sellButton.onClick.AddListener(OnSellButtonClicked);

        UpdateDemandBarColor(itemCost);

        itemImageComponent = GetComponent<Image>();
        if (itemImageComponent == null)
        {
            Debug.LogError("[InventoryItemUI] Image component not found on " + itemName);
        }

        priceInputField.text = itemCost.ToString("F2");
        priceInputField.onValueChanged.AddListener(delegate { UpdateDemandBarBasedOnPrice(); });
    }

    /// <summary>
    /// Updates the demand bar based on price.
    /// </summary>
    private void UpdateDemandBarBasedOnPrice()
    {
        if (float.TryParse(priceInputField.text, out float enteredPrice))
        {
            Color demandColor = InventoryManager.CalculateDemandBarColor(enteredPrice, originalCost);
            SetDemandBarColor(demandColor);
        }
    }


    /// <summary>
    /// Updates the color of the demand bar.
    /// </summary>
    /// <param name="currentPrice">The current price.</param>
    private void UpdateDemandBarColor(float currentPrice)
    {
        float lowerBound = originalCost; // Green at the base cost
        float idealPrice = originalCost * 1.25f; // Ideal price at + 25% of the original cost
        float upperBound = originalCost * 1.5f;// Red at 50% increase of original price

        //Normalize the current price within the range
        float normalizedValue = Mathf.InverseLerp(lowerBound, upperBound, currentPrice);

        // Interpolate the colour
        Color demandColor;
        if (currentPrice <= idealPrice)
        {
            // Interpolate between green and yellow towards the ideal price
            demandColor = Color.Lerp(Color.green, Color.yellow, normalizedValue * 2); // Multiplied by 2 because it's half the range
        }
        else
        {
            // Interpolate between yellow and red past the ideal price
            demandColor = Color.Lerp(Color.yellow, Color.red, (normalizedValue - 0.5f) * 2); // Adjusted range
        }
        SetDemandBarColor(demandColor);

    }

    /// <summary>
    /// Increments the quantity.
    /// </summary>
    public void IncrementQuantity()
    {
        Debug.Log($"[InventoryItemUI] Attempting to increment quantity for {itemName}. Current sellQuantity: {sellQuantity}, Available Quantity: {quantity}");
        if (sellQuantity < quantity)
        {
            sellQuantity++;
            Debug.Log($"[InventoryItemUI] Incremented sellQuantity for {itemName} to {sellQuantity}");
            UpdateUI();
        }
        else
        {
            Debug.Log($"[InventoryItemUI] Max sell quantity reached for {itemName}");
        }
    }

    /// <summary>
    /// Decrements the quantity.
    /// </summary>
    public void DecrementQuantity()
    {
        Debug.Log($"[InventoryItemUI] Attempting to decrement quantity for {itemName}. Current sellQuantity: {sellQuantity}");
        if (sellQuantity > 0)
        {
            sellQuantity--;
            Debug.Log($"[InventoryItemUI] Decremented sellQuantity for {itemName} to {sellQuantity}");
            UpdateUI();
        }
        else
        {
            Debug.Log($"[InventoryItemUI] No quantity to decrement for {itemName}");
        }
    }



    /// <summary>
    /// Updates the UI.
    /// </summary>
    public void UpdateUI()
    {
        Debug.Log($"[InventoryItemUI] Updating UI for {itemName}, Available Quantity: {quantity}, Sell Quantity: {sellQuantity}");

        // Ensure that the itemNameText is updated correctly
        TextMeshProUGUI itemNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }
        else
        {
            Debug.LogError("[InventoryItemUI] Failed to find 'NameText' component in UI.");
        }

        // Update the quantity text
        TextMeshProUGUI quantityText = transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
        }
        else
        {
            Debug.LogError("[InventoryItemUI] Failed to find 'QuantityText' component in UI.");
        }

        // Update the sell quantity text, which seems to be missing in your current method
        TextMeshProUGUI sellQuantityText = transform.Find("SellingQuantity").GetComponent<TextMeshProUGUI>();
        if (sellQuantityText != null)
        {
            sellQuantityText.text = sellQuantity.ToString();
        }
        else
        {
            Debug.LogError("[InventoryItemUI] Failed to find 'SellingQuantity' component in UI.");
        }

        // Update the item image
        Image itemImageComponent = GetComponent<Image>();
        if (itemImageComponent != null)
        {
            InventoryItem correspondingItem = inventoryManager.inventoryItems.Find(item => item.itemName == itemName);
            if (correspondingItem != null && correspondingItem.itemImage != null)
            {
                itemImageComponent.sprite = correspondingItem.itemImage;
            }
            else
            {
                Debug.LogError("[InventoryItemUI] ItemImage component or sprite is null or not found.");
            }
        }
        else
        {
            Debug.LogError("[InventoryItemUI] ItemImage component not found in UI.");
        }
    }




}
