using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a customer in the game.
/// </summary>
public class Customer : MonoBehaviour
{
    /// <summary>
    /// The list of items the customer desires.
    /// </summary>
    private List<string> desiredItems = new List<string>();

    /// <summary>
    /// The list of items the customer has found.
    /// </summary>
    private List<string> foundItems = new List<string>();

    /// <summary>
    /// The shelf manager to interact with the shelves.
    /// </summary>
    private ShelfManager shelfManager;


    /// <summary>
    /// The list of items the customer has purchased.
    /// </summary>
    public List<PurchasedItem> purchasedItems = new List<PurchasedItem>();

    /// <summary>
    /// The customer's budget.
    /// </summary>
    public float budget = 50.0f;

    /// <summary>
    /// The customer's tolerance for price increases.
    /// </summary>
    public float priceIncreaseTolerance;

    /// <summary>
    /// The customer's name.
    /// </summary>
    public string customerName;

    /// <summary>
    /// The customer's feedback.
    /// </summary>
    public string feedback;

    /// <summary>
    /// The number of items in the customer's basket.
    /// </summary>
    public int itemsInBasket;

    /// <summary>
    /// Indicates whether the customer has made a purchase today.
    /// </summary>
    public bool hasPurchasedToday { get; set; }

    /// <summary>
    /// The prefab for the customer shopping UI.
    /// </summary>
    public GameObject customerShoppingPrefab;

    /// <summary>
    /// The panel for the till background.
    /// </summary>
    public GameObject tillBGPanel;

    /// <summary>
    /// The parent transform for the shopping background.
    /// </summary>
    public Transform shoppingBGParent;

    /// <summary>
    /// The TextMeshPro component for feedback text.
    /// </summary>
    public TextMeshProUGUI feedbackText;

    /// <summary>
    /// The TextMeshPro component for items text.
    /// </summary>
    public TextMeshProUGUI itemsText;

    /// <summary>
    /// The TextMeshPro component for name text.
    /// </summary>
    public TextMeshProUGUI nameText;

    /// <summary>
    /// Array of possible first initials for customer names.
    /// </summary>
    string[] firstInitials = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    /// <summary>
    /// Array of possible surnames for customer names.
    /// </summary>
    string[] surnames = new string[] {"Casey", "Podd", "Symonds", "Smith", "Jones", "Baker", "Fry", "Janes", "Thomas", "Bristow", "Williams", "Wilson", "Taylor", "Brown",
        "Johnson", "Evans", "Roberts", "Edwards", "Hughes", "Walker", "Davies", "Robinson", "Green", "Thompson", "Wright", "Wood", "Clark", "Clarke", "Anderson", "Campbell",
        "Martin", "Lewis", "Harris", "Jackson", "Patel", "Turner", "Cooper", "Hill", "Ward", "Morris", "Moore", "Lee", "King", "Harrison", "Morgan", "Allen", "James", "Scott",
        "Phillips", "Watson", "Parker", "Price", "Bennett", "Young", "Griffiths", "Mitchell", "Kelly", "Cook", "Carter", "Richardson", "Bailey", "Collins", "Bell", "Shaw",
        "Murphy", "Miller", "Cox", "Khan", "Richards", "Marshall", "Simpson", "Ellis", "Adams", "Singh", "Begum", "Wilkinson", "Foster", "Chapman", "Powell", "Webb", "Rogers",
        "Gray", "Mason", "Ali", "Hunt", "Hussain", "Owen", "Palmer", "Holmes", "Barnes", "Knight", "Lloyd", "Butler", "Russell", "Fisher", "Barker", "Stevens", "Jenkins",
        "Dixon", "Fletcher"};


    /// <summary>
    /// Initializes the customer.
    /// </summary>
    private void Awake()
    {
        shelfManager = FindObjectOfType<ShelfManager>();
        InitializeDesiredItems();
        InitializePriceTolerance();
        hasPurchasedToday = false;
    }

    /// <summary>
    /// Checks if the specified item is available on the shelves.
    /// </summary>
    /// <param name="itemName">The name of the item to check.</param>
    /// <returns>True if the item is available, false otherwise.</returns>
    bool CheckIfItemIsAvailable(string itemName)
    {
        foreach (var shelfItemGO in shelfManager.shelfItems)
        {
            ShelfItemUI shelfItem = shelfItemGO.GetComponent<ShelfItemUI>(); 
            if (shelfItem != null && shelfItem.itemName == itemName && shelfItem.quantityOnShelf > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the specified item is too expensive for the customer.
    /// </summary>
    /// <param name="itemName">The name of the item to check.</param>
    /// <returns>True if the item is too expensive, false otherwise.</returns>
    bool CheckIfItemTooExpensive(string itemName)
    {
        ShelfItemUI item = shelfManager.shelfItems
            .Select(si => si.GetComponent<ShelfItemUI>())
            .FirstOrDefault(siUI => siUI.itemName == itemName);

        return item != null && item.sellingPrice > (item.inventoryItem.cost * 1.25f); // Adjust threshold if needed
    }

    /// <summary>
    /// Chooses a random item from the list of available items.
    /// </summary>
    /// <param name="availableItems">The list of available items.</param>
    /// <returns>The chosen item.</returns>
    private InventoryItem ChooseItem(List<InventoryItem> availableItems)
    {
        if (availableItems.Count > 0)
        {
            return availableItems[Random.Range(0, availableItems.Count)];
        }
        return null;
    }

    /// <summary>
    /// Determines if the customer will continue shopping.
    /// </summary>
    /// <returns>True if the customer will continue shopping, false otherwise.</returns>
    private bool ContinueShopping()
    {
        return Random.value > 0.2f; // 80% chance to continue shopping after each purchase
    }

    /// <summary>
    /// Evaluates and selects items for purchase.
    /// </summary>
    void EvaluateAndSelectItems()
    {
        int itemsNotFound = 0; // Tracks items not found
        float totalSpent = 0; // Total amount spent by the customer
        float totalProfit = 0; // Total profit from this customer's purchases

        foreach (var desiredItemName in desiredItems)
        {
            var shelfItem = shelfManager.shelfItems
                .Select(si => si.GetComponent<ShelfItemUI>())
                .FirstOrDefault(siUI => siUI != null && siUI.itemName == desiredItemName && siUI.quantityOnShelf > 0);

            if (shelfItem != null)
            {
                bool isPriceAcceptable = shelfItem.sellingPrice <= shelfItem.inventoryItem.cost * priceIncreaseTolerance;
                if (isPriceAcceptable && budget >= shelfItem.sellingPrice)
                {
                    itemsInBasket++;
                    budget -= shelfItem.sellingPrice;
                    shelfItem.quantityOnShelf--;

                    float profit = shelfItem.sellingPrice - shelfItem.inventoryItem.cost;
                    totalSpent += shelfItem.sellingPrice;
                    totalProfit += profit;

                    purchasedItems.Add(new PurchasedItem { itemName = shelfItem.itemName, quantity = 1, price = shelfItem.sellingPrice, profitPerItem = profit });

                    shelfItem.UpdateUI();
                }
                else
                {
                    itemsNotFound++; // Track this as a missed opportunity due to price/budget constraints
                }
            }
            else
            {
                itemsNotFound++; // Item not found at all
            }
        }

        // Debug log to display what the customer has bought, the total spent, and the total profit
        if (purchasedItems.Any())
        {
            Debug.Log($"Customer: {customerName} bought {purchasedItems.Count} items, spent a total of £{totalSpent:F2}, resulting in a total profit of £{totalProfit:F2}.");
        }

        if (itemsNotFound > 0)
        {
            FindObjectOfType<DailySummaryManager>().RegisterStockShortage(1, itemsNotFound); // Notify the DailySummaryManager for each customer and the number of items not found
        }
    }

    /// <summary>
    /// Finds a ShelfItemUI by item name.
    /// </summary>
    /// <param name="shelfManager">The shelf manager to search within.</param>
    /// <param name="itemName">The name of the item to find.</param>
    /// <returns>The ShelfItemUI if found, null otherwise.</returns>
    private ShelfItemUI FindShelfItemUI(ShelfManager shelfManager, string itemName)
    {
        foreach (var shelfItem in shelfManager.shelfItems)
        {
            ShelfItemUI shelfItemUI = shelfItem.GetComponent<ShelfItemUI>();
            if (shelfItemUI.itemName == itemName)
            {
                return shelfItemUI;
            }
        }
        return null; // No item found
    }

    /// <summary>
    /// Generates feedback based on the customer's shopping experience.
    /// </summary>
    void GenerateFeedback()
    {
        string positiveFeedback = "";
        string negativeFeedback = "";

        // Setup for color-coded feedback
        string greenColorStartTag = "<color=#07D138>";
        string redColorStartTag = "<color=#FF615D>";
        string colorEndTag = "</color>";

        HashSet<string> itemsConsideredExpensive = new HashSet<string>();

        // Identify items considered too expensive
        foreach (var item in desiredItems)
        {
            if (CheckIfItemTooExpensive(item) && !purchasedItems.Any(pi => pi.itemName == item))
            {
                itemsConsideredExpensive.Add(item);
            }
        }

        // Positive feedback for purchased items
        if (purchasedItems.Any())
        {
            var randomPurchasedItem = purchasedItems[UnityEngine.Random.Range(0, purchasedItems.Count)];
            positiveFeedback = greenColorStartTag + $"Got {randomPurchasedItem.itemName}" + colorEndTag;
        }

        // Select one item for negative feedback if it was not purchased for being too expensive or not found
        var itemsNotPurchased = desiredItems.Except(purchasedItems.Select(pi => pi.itemName)).Except(itemsConsideredExpensive).ToList();
        if (itemsConsideredExpensive.Count > 0)
        {
            var randomExpensiveItem = itemsConsideredExpensive.FirstOrDefault();
            negativeFeedback = redColorStartTag + $"{randomExpensiveItem} was too expensive" + colorEndTag;
        }
        else if (itemsNotPurchased.Count > 0)
        {
            var randomNotPurchasedItem = itemsNotPurchased[UnityEngine.Random.Range(0, itemsNotPurchased.Count)];
            negativeFeedback = redColorStartTag + $"Couldn't find any {randomNotPurchasedItem}" + colorEndTag;
        }

        // Combining feedback
        feedback = positiveFeedback;
        if (!string.IsNullOrEmpty(negativeFeedback))
        {
            feedback += (string.IsNullOrEmpty(positiveFeedback) ? "" : " ") + negativeFeedback;
        }

        UpdateUI();

        // Calculate and update customer satisfaction based on feedback
        UpdateCustomerSatisfaction(itemsConsideredExpensive.Count, itemsNotPurchased.Count);
    }

    /// <summary>
    /// Updates the customer's satisfaction based on their shopping experience.
    /// </summary>
    /// <param name="expensiveItemsCount">The number of items considered too expensive.</param>
    /// <param name="notPurchasedItemsCount">The number of items not purchased.</param>
    void UpdateCustomerSatisfaction(int expensiveItemsCount, int notPurchasedItemsCount)
    {
        float satisfactionChange = 0f;
        if (expensiveItemsCount > 0)
        {
            satisfactionChange -= expensiveItemsCount * 0.5f; // Decrease satisfaction for each too expensive item
        }
        if (notPurchasedItemsCount > 0)
        {
            satisfactionChange -= notPurchasedItemsCount * 0.2f; // Decrease satisfaction for each not purchased item
        }
        if (purchasedItems.Count > 0)
        {
            satisfactionChange += purchasedItems.Count * 0.3f; // Increase satisfaction for each purchased item
        }

        // Find DailySummaryManager and update daily satisfaction
        DailySummaryManager dailySummaryManager = FindObjectOfType<DailySummaryManager>();
        if (dailySummaryManager != null)
        {
            dailySummaryManager.UpdateDailyCustomerSatisfaction(satisfactionChange);
        }
    }

    /// <summary>
    /// Generates a random customer name.
    /// </summary>
    /// <returns>The generated customer name.</returns>
    string GenerateRandomName()
    {
        string initial = firstInitials[Random.Range(0, firstInitials.Length)];
        string surname = surnames[Random.Range(0, surnames.Length)];
        return initial + ". " + surname;
    }

    /// <summary>
    /// Gets the price of an item from the shelf manager.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <returns>The price of the item.</returns>
    float GetItemPrice(string itemName)
    {
        return shelfManager.GetItemPrice(itemName); // Ensure this method exists in ShelfManager
    }

    /// <summary>
    /// Sends the customer to the till.
    /// </summary>
    void GoToTill()
    {
        TillManager tillManager = FindObjectOfType<TillManager>();
        if (tillManager != null)
        {
            tillManager.AddCustomerToQueue(this);
            FindObjectOfType<CustomerSpawner>().CustomerExited(); // Signal when customer goes to till
        }
    }

    /// <summary>
    /// Initializes the customer's desired items.
    /// </summary>
    private void InitializeDesiredItems()
    {
        WholesaleManager wholesaleManager = FindObjectOfType<WholesaleManager>();
        if (wholesaleManager != null && wholesaleManager.wholesaleItems.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(10, wholesaleManager.wholesaleItems.Count); i++)
            {
                InventoryItem randomItem = wholesaleManager.wholesaleItems[Random.Range(0, wholesaleManager.wholesaleItems.Count)];
                if (!desiredItems.Contains(randomItem.itemName))
                {
                    desiredItems.Add(randomItem.itemName);
                }
            }
        }
    }

    /// <summary>
    /// Initializes the customer's price tolerance.
    /// </summary>
    private void InitializePriceTolerance()
    {
        float chance = Random.Range(0f, 100f);
        if (chance <= 80) // 80% chance
        {
            priceIncreaseTolerance = Random.Range(1.25f, 1.35f); // Standard tolerance (25% to 35%)
        }
        else if (chance <= 90) // 10% chance
        {
            priceIncreaseTolerance = Random.Range(1.2f, 1.25f); // Lower tolerance (20% to 25%)
        }
        else // 10% chance
        {
            priceIncreaseTolerance = Random.Range(1.35f, 2f); // Higher tolerance (35% to 100%)
        }
    }

    /// <summary>
    /// Checks if the specified item is desired by the customer.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <returns>True if the item is desired, false otherwise.</returns>
    private bool IsItemDesired(string itemName)
    {
        return desiredItems.Contains(itemName);
    }

    /// <summary>
    /// Checks if the specified price is acceptable for the customer.
    /// </summary>
    /// <param name="price">The price of the item.</param>
    /// <returns>True if the price is acceptable, false otherwise.</returns>
    private bool IsPriceAcceptable(float price)
    {
        return price <= budget; // Simple check against the budget
    }

    /// <summary>
    /// Removes the customer if they have not made any purchases.
    /// </summary>
    /// <returns>Coroutine for removing the customer.</returns>
    private IEnumerator RemoveCustomerIfNoPurchase()
    {
        yield return new WaitForSeconds(4f); // Adjusted time to ensure shopping can complete

        if (itemsInBasket == 0)
        {
            Debug.Log($"Removing customer: {customerName} due to 0 items in basket.");
            InformationBar.Instance.DisplayMessage($"{customerName} left without purchasing.");
            FindObjectOfType<CustomerSpawner>().CustomerExited(); // Customer exits without purchase
            Destroy(gameObject); // Remove this customer object if they haven't selected any items
        }
        else
        {
            GoToTill();
        }
    }

    /// <summary>
    /// Initializes the customer and starts their shopping routine.
    /// </summary>
    private void Start()
    {
        // Signal that a new customer has started shopping
        FindObjectOfType<CustomerSpawner>().CustomerEntered();
        StartCoroutine(StartShoppingRoutine());
        customerName = GenerateRandomName();
        itemsInBasket = 0;
        UpdateUI();
    }

    /// <summary>
    /// Starts the customer's shopping routine.
    /// </summary>
    /// <returns>Coroutine for the shopping routine.</returns>
    IEnumerator StartShoppingRoutine()
    {
        yield return new WaitForSeconds(2f);
        EvaluateAndSelectItems();
        GenerateFeedback();
        StartCoroutine(RemoveCustomerIfNoPurchase());
    }

    /// <summary>
    /// Updates the customer's UI.
    /// </summary>
    void UpdateUI()
    {
        nameText.text = customerName;
        itemsText.text = itemsInBasket.ToString();
        feedbackText.text = feedback; 
    }

    /// <summary>
    /// Adds the specified amount of items to the customer's basket.
    /// </summary>
    /// <param name="amount">The amount of items to add.</param>
    public void AddToBasket(int amount)
    {
        itemsInBasket += amount;
        UpdateUI();
    }

    /// <summary>
    /// Displays the customer's shopping information.
    /// </summary>
    public void DisplayShoppingInfo()
    {
        // Ensure there's a prefab and parent assigned
        if (customerShoppingPrefab != null && shoppingBGParent != null)
        {
            // Instantiate the shopping prefab under the ShoppingBG parent
            GameObject shoppingInfoUI = Instantiate(customerShoppingPrefab, shoppingBGParent);

            // Find the Text components in the instantiated prefab and update them
            shoppingInfoUI.transform.Find("CustomerNameText").GetComponent<TextMeshProUGUI>().text = customerName;
            shoppingInfoUI.transform.Find("ItemsInBasketText").GetComponent<TextMeshProUGUI>().text = $"Items: {itemsInBasket}";
            shoppingInfoUI.transform.Find("FeedbackText").GetComponent<TextMeshProUGUI>().text = feedback;
        }
    }

    /// <summary>
    /// Gets the cost of an item from the shelf manager.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <returns>The cost of the item.</returns>
    public float GetItemCost(string itemName)
    {
        return shelfManager.GetItemCost(itemName);
    }

    /// <summary>
    /// Gets the list of items the customer has purchased.
    /// </summary>
    /// <returns>The list of purchased items.</returns>
    public List<PurchasedItem> GetPurchasedItems()
    {
        return purchasedItems;
    }

    /// <summary>
    /// Makes a purchase decision based on available items and budget.
    /// </summary>
    /// <param name="shelfManager">The shelf manager to interact with.</param>
    public void MakePurchaseDecision(ShelfManager shelfManager)
    {
        // Simulate a shopping list: Choose a random subset of items from the wholesale list as the customer's desired items.
        List<InventoryItem> shoppingList = new List<InventoryItem>();
        foreach (var item in shelfManager.shelfItems) 
        {
            if (Random.value > 0.5f) // Randomly decide if this item is on the customer's shopping list
            {
                shoppingList.Add(item.GetComponent<ShelfItemUI>().inventoryItem);
            }
        }

        // Attempt to buy items from the shopping list based on availability and budget
        foreach (var item in shoppingList)
        {
            ShelfItemUI shelfItemUI = FindShelfItemUI(shelfManager, item.itemName);
            if (shelfItemUI != null && budget >= shelfItemUI.sellingPrice && shelfItemUI.quantityOnShelf > 0)
            {
                int quantityToBuy = Mathf.Min((int)(budget / shelfItemUI.sellingPrice), shelfItemUI.quantityOnShelf);
                budget -= quantityToBuy * shelfItemUI.sellingPrice;
                itemsInBasket += quantityToBuy;
                shelfItemUI.quantityOnShelf -= quantityToBuy; // Update shelf quantity
                shelfItemUI.UpdateUI();

                ProvideFeedback($"Bought {quantityToBuy} of {item.itemName}.");
                if (budget < shelfItemUI.sellingPrice) break; // Exit loop if budget is spent
            }
        }

        if (itemsInBasket == 0)
        {
            ProvideFeedback("Didn't find what I was looking for.");
        }
    }

    /// <summary>
    /// Provides feedback to the customer.
    /// </summary>
    /// <param name="message">The feedback message.</param>
    public void ProvideFeedback(string message)
    {
        feedback = message;
        UpdateUI();
    }

    /// <summary>
    /// Represents an item the customer has purchased.
    /// </summary>
    [System.Serializable]
    public class PurchasedItem
    {
        /// <summary>
        /// The name of the purchased item.
        /// </summary>
        public string itemName;

        /// <summary>
        /// The price of the purchased item.
        /// </summary>
        public float price;

        /// <summary>
        /// The profit per item.
        /// </summary>
        public float profitPerItem;

        /// <summary>
        /// The quantity of the purchased item.
        /// </summary>
        public int quantity;
    }
}