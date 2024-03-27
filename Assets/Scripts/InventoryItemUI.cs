using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image demandBar;
    public Button plusButton;
    public Button minusButton;
    public TMP_InputField priceInputField;
    public TextMeshProUGUI sellQuantityText;
    public Button sellButton;

    [Header("Item Settings")]
    public string itemName;
    public float itemCost;
    public int quantity = 0;
    public int sellQuantity = 0;

    [Header("Manager References")]
    public WholesaleManager wholesaleManager;
    public InventoryManager inventoryManager;

    [Header("Misc")]
    public MaterialPropertyBlock demandBarMaterial;
    public bool isSelectedForSelling = false;

    // Private fields and methods
    private float sellingPrice = 0.0f;
    private Image itemImageComponent;
    private float originalCost;


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

    private void Start()
    {
        // Store the original cost when initializing the item
        originalCost = CalculateOriginalCost();

        // Add listeners to the plus and minus buttons
        plusButton.onClick.AddListener(() => IncrementQuantity()); // Lambda function with no arguments
        minusButton.onClick.AddListener(DecrementQuantity);
        sellButton.onClick.AddListener(OnSellButtonClicked);

        // Update the DemandBar color initially
        UpdateDemandBarColor(itemCost);

        // Get the Image component attached to this GameObject
        itemImageComponent = GetComponent<Image>();

        priceInputField.text = itemCost.ToString("F2"); // Initialized with the base cost of the item
        priceInputField.onValueChanged.AddListener(delegate { UpdateDemandBarBasedOnPrice(); });

    }


    public void UpdateUI()
    {
        // Set the text for the item name
        TextMeshProUGUI itemNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        itemNameText.text = itemName;

        // Set the text for the quantity to sell
        TextMeshProUGUI quantityText = transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();
        quantityText.text = quantity.ToString();

        // Set the text for the sell quantity
        sellQuantityText.text = sellQuantity.ToString(); // Update the sellQuantityText

        // Set the text for the cost
        TextMeshProUGUI costText = transform.Find("BoughtForText").GetComponent<TextMeshProUGUI>();
        costText.text = "Cost Each £" + itemCost.ToString("F2");

        // Get the Image component attached to this GameObject
        Image itemImageComponent = GetComponent<Image>();

        // Find the corresponding InventoryItem with the same itemName
        InventoryItem correspondingItem = inventoryManager.inventoryItems.Find(item => item.itemName == itemName);

        // Update the item image
        if (itemImageComponent != null && correspondingItem != null)
        {
            itemImageComponent.sprite = correspondingItem.itemImage;
        }

        // Update the demand bar color based on the current itemCost
        UpdateDemandBarColor(itemCost);
        UpdateDemandBarBasedOnPrice();
    }


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

    public void IncrementQuantity()
    {
        // Check if the player has this item in their inventory
        if (sellQuantity < quantity)
        {
            sellQuantity++;
            UpdateUI();
        }
        else
        {
            Debug.Log("You can't have more of this item.");
        }
    }


    public void DecrementQuantity()
    {
        if (sellQuantity > 0)
        {
            sellQuantity--;
            UpdateUI();
        }
        else
        {
            Debug.Log("Sell quantity is already at zero.");
        }
    }



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

    private void UpdateDemandBarBasedOnPrice()
    {
        if (float.TryParse(priceInputField.text, out float enteredPrice))
        {
            Color demandColor = InventoryManager.CalculateDemandBarColor(enteredPrice, originalCost);
            SetDemandBarColor(demandColor);
        }
    }

    void OnSellButtonClicked()
    {
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
}
