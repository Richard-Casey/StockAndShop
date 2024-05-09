using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class Customer : MonoBehaviour
{
    /// <summary>
    /// The desired items
    /// </summary>
    private List<string> desiredItems = new List<string>();
    /// <summary>
    /// The found items
    /// </summary>
    private List<string> foundItems = new List<string>();
    /// <summary>
    /// The shelf manager
    /// </summary>
    private ShelfManager shelfManager;

    /// <summary>
    /// The purchased items
    /// </summary>
    public List<PurchasedItem> purchasedItems = new List<PurchasedItem>();
    /// <summary>
    /// The budget
    /// </summary>
    public float budget = 50.0f;
    /// <summary>
    /// The price increase tolerance
    /// </summary>
    public float priceIncreaseTolerance;
    /// <summary>
    /// The customer name
    /// </summary>
    public string customerName;
    /// <summary>
    /// The feedback
    /// </summary>
    public string feedback;
    /// <summary>
    /// The items in basket
    /// </summary>
    public int itemsInBasket;
    /// <summary>
    /// Gets or sets a value indicating whether this instance has purchased today.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has purchased today; otherwise, <c>false</c>.
    /// </value>
    public bool hasPurchasedToday { get; set; }
    /// <summary>
    /// The customer shopping prefab
    /// </summary>
    public GameObject customerShoppingPrefab;
    /// <summary>
    /// The till bg panel
    /// </summary>
    public GameObject tillBGPanel;
    /// <summary>
    /// The shopping bg parent
    /// </summary>
    public Transform shoppingBGParent;
    /// <summary>
    /// The feedback text
    /// </summary>
    public TextMeshProUGUI feedbackText;
    /// <summary>
    /// The items text
    /// </summary>
    public TextMeshProUGUI itemsText;
    /// <summary>
    /// The name text
    /// </summary>
    public TextMeshProUGUI nameText;
    /// <summary>
    /// The first initials
    /// </summary>
    string[] firstInitials = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    /// <summary>
    /// The surnames
    /// </summary>
    string[] surnames = new string[] {"Casey", "Podd", "Symonds", "Smith", "Jones", "Baker", "Fry", "Janes", "Thomas", "Bristow", "Williams", "Wilson", "Taylor", "Brown",
        "Johnson", "Evans", "Roberts", "Edwards", "Hughes", "Walker", "Davies", "Robinson", "Green", "Thompson", "Wright", "Wood", "Clark", "Clarke", "Anderson", "Campbell",
        "Martin", "Lewis", "Harris", "Jackson", "Patel", "Turner", "Cooper", "Hill", "Ward", "Morris", "Moore", "Lee", "King", "Harrison", "Morgan", "Allen", "James", "Scott",
        "Phillips", "Watson", "Parker", "Price", "Bennett", "Young", "Griffiths", "Mitchell", "Kelly", "Cook", "Carter", "Richardson", "Bailey", "Collins", "Bell", "Shaw",
        "Murphy", "Miller", "Cox", "Khan", "Richards", "Marshall", "Simpson", "Ellis", "Adams", "Singh", "Begum", "Wilkinson", "Foster", "Chapman", "Powell", "Webb", "Rogers",
        "Gray", "Mason", "Ali", "Hunt", "Hussain", "Owen", "Palmer", "Holmes", "Barnes", "Knight", "Lloyd", "Butler", "Russell", "Fisher", "Barker", "Stevens", "Jenkins",
        "Dixon", "Fletcher"};

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        shelfManager = FindObjectOfType<ShelfManager>();
        InitializeDesiredItems();
        InitializePriceTolerance();
        hasPurchasedToday = false;
    }

    /// <summary>
    /// Checks if item is available.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    bool CheckIfItemIsAvailable(string itemName)
    {
        // Assuming each shelf item GameObject has a ShelfItemUI component attached
        // that holds the itemName and quantityOnShelf properties
        foreach (var shelfItemGO in shelfManager.shelfItems)
        {
            ShelfItemUI shelfItem = shelfItemGO.GetComponent<ShelfItemUI>(); // Adjust ShelfItemUI to your actual component class name
            if (shelfItem != null && shelfItem.itemName == itemName && shelfItem.quantityOnShelf > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if item too expensive.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    bool CheckIfItemTooExpensive(string itemName)
    {
        ShelfItemUI item = shelfManager.shelfItems
            .Select(si => si.GetComponent<ShelfItemUI>())
            .FirstOrDefault(siUI => siUI.itemName == itemName);

        return item != null && item.sellingPrice > (item.inventoryItem.cost * 1.25f); // Adjust threshold as needed
    }

    /// <summary>
    /// Chooses the item.
    /// </summary>
    /// <param name="availableItems">The available items.</param>
    /// <returns></returns>
    private InventoryItem ChooseItem(List<InventoryItem> availableItems)
    {
        // Implement your logic here. For now, return a random item
        if (availableItems.Count > 0)
        {
            return availableItems[Random.Range(0, availableItems.Count)];
        }
        return null;
    }

    /// <summary>
    /// Continues the shopping.
    /// </summary>
    /// <returns></returns>
    private bool ContinueShopping()
    {
        return Random.value > 0.2f; // 80% chance to continue shopping after each purchase
    }

    /// <summary>
    /// Evaluates the and select items.
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

    // Helper method to find a ShelfItemUI by item name
    /// <summary>
    /// Finds the shelf item UI.
    /// </summary>
    /// <param name="shelfManager">The shelf manager.</param>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
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
    /// Generates the feedback.
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
    /// Updates the customer satisfaction.
    /// </summary>
    /// <param name="expensiveItemsCount">The expensive items count.</param>
    /// <param name="notPurchasedItemsCount">The not purchased items count.</param>
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
    /// Generates the random name.
    /// </summary>
    /// <returns></returns>
    string GenerateRandomName()
    {
        string initial = firstInitials[Random.Range(0, firstInitials.Length)];
        string surname = surnames[Random.Range(0, surnames.Length)];
        return initial + ". " + surname;
    }

    /// <summary>
    /// Gets the item price.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    float GetItemPrice(string itemName)
    {
        return shelfManager.GetItemPrice(itemName); // Ensure this method exists in ShelfManager
    }

    /// <summary>
    /// Goes to till.
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
    /// Initializes the desired items.
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
    /// Initializes the price tolerance.
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
    /// Determines whether [is item desired] [the specified item name].
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns>
    ///   <c>true</c> if [is item desired] [the specified item name]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsItemDesired(string itemName)
    {
        return desiredItems.Contains(itemName);
    }

    /// <summary>
    /// Determines whether [is price acceptable] [the specified price].
    /// </summary>
    /// <param name="price">The price.</param>
    /// <returns>
    ///   <c>true</c> if [is price acceptable] [the specified price]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsPriceAcceptable(float price)
    {
        return price <= budget; // Simple check against the budget
    }

    /// <summary>
    /// Removes the customer if no purchase.
    /// </summary>
    /// <returns></returns>
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
    /// Starts this instance.
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
    /// Starts the shopping routine.
    /// </summary>
    /// <returns></returns>
    IEnumerator StartShoppingRoutine()
    {
        yield return new WaitForSeconds(2f);
        EvaluateAndSelectItems();
        GenerateFeedback();
        StartCoroutine(RemoveCustomerIfNoPurchase());
    }

    /// <summary>
    /// Updates the UI.
    /// </summary>
    void UpdateUI()
    {
        nameText.text = customerName;
        itemsText.text = itemsInBasket.ToString();
        feedbackText.text = feedback; // This will now include the color tags
    }

    /// <summary>
    /// Adds to basket.
    /// </summary>
    /// <param name="amount">The amount.</param>
    public void AddToBasket(int amount)
    {
        itemsInBasket += amount;
        UpdateUI();
    }

    /// <summary>
    /// Displays the shopping information.
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
    /// Gets the item cost.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    public float GetItemCost(string itemName)
    {
        // Implementation depends on how ShelfManager stores item costs
        return shelfManager.GetItemCost(itemName);
    }

    /// <summary>
    /// Gets the purchased items.
    /// </summary>
    /// <returns></returns>
    public List<PurchasedItem> GetPurchasedItems()
    {
        return purchasedItems;
    }

    /// <summary>
    /// Makes the purchase decision.
    /// </summary>
    /// <param name="shelfManager">The shelf manager.</param>
    public void MakePurchaseDecision(ShelfManager shelfManager)
    {
        // Simulate a shopping list: Choose a random subset of items from the wholesale list as the customer's desired items.
        List<InventoryItem> shoppingList = new List<InventoryItem>();
        foreach (var item in shelfManager.shelfItems) // Assuming shelfItems now holds InventoryItem references or similar.
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
    /// Provides the feedback.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ProvideFeedback(string message)
    {
        feedback = message;
        UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class PurchasedItem
    {
        /// <summary>
        /// The item name
        /// </summary>
        public string itemName;
        /// <summary>
        /// The price
        /// </summary>
        public float price;
        /// <summary>
        /// The profit per item
        /// </summary>
        public float profitPerItem;
        /// <summary>
        /// The quantity
        /// </summary>
        public int quantity;
    }
}