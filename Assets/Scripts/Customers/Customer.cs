using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Customer : MonoBehaviour
{
    private List<string> desiredItems = new List<string>();

    string[] firstInitials = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    private List<string> foundItems = new List<string>();
    private ShelfManager shelfManager;
    string[] surnames = new string[] {"Casey", "Podd", "Symonds", "Smith", "Jones", "Baker", "Fry", "Janes", "Thomas", "Bristow", "Williams", "Wilson", "Taylor", "Brown",
        "Johnson", "Evans", "Roberts", "Edwards", "Hughes", "Walker", "Davies", "Robinson", "Green", "Thompson", "Wright", "Wood", "Clark", "Clarke", "Anderson", "Campbell",
        "Martin", "Lewis", "Harris", "Jackson", "Patel", "Turner", "Cooper", "Hill", "Ward", "Morris", "Moore", "Lee", "King", "Harrison", "Morgan", "Allen", "James", "Scott",
        "Phillips", "Watson", "Parker", "Price", "Bennett", "Young", "Griffiths", "Mitchell", "Kelly", "Cook", "Carter", "Richardson", "Bailey", "Collins", "Bell", "Shaw",
        "Murphy", "Miller", "Cox", "Khan", "Richards", "Marshall", "Simpson", "Ellis", "Adams", "Singh", "Begum", "Wilkinson", "Foster", "Chapman", "Powell", "Webb", "Rogers",
        "Gray", "Mason", "Ali", "Hunt", "Hussain", "Owen", "Palmer", "Holmes", "Barnes", "Knight", "Lloyd", "Butler", "Russell", "Fisher", "Barker", "Stevens", "Jenkins",
        "Dixon", "Fletcher"};
    public float budget = 50.0f;
    // Attributes
    public string customerName;
    public GameObject customerShoppingPrefab;
    public string feedback;
    public TextMeshProUGUI feedbackText;
    public int itemsInBasket;
    public TextMeshProUGUI itemsText;

    // Reference to UI text elements
    public TextMeshProUGUI nameText;
    public float priceIncreaseTolerance;

    public List<PurchasedItem> purchasedItems = new List<PurchasedItem>();
    public Transform shoppingBGParent;
    public GameObject tillBGPanel;



    private void Awake()
    {
        shelfManager = FindObjectOfType<ShelfManager>();
        InitializeDesiredItems();
        InitializePriceTolerance();
    }






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

    bool CheckIfItemTooExpensive(string itemName)
    {
        ShelfItemUI item = shelfManager.shelfItems
            .Select(si => si.GetComponent<ShelfItemUI>())
            .FirstOrDefault(siUI => siUI.itemName == itemName);

        return item != null && item.sellingPrice > (item.inventoryItem.cost * 1.25f); // Adjust threshold as needed
    }


    private InventoryItem ChooseItem(List<InventoryItem> availableItems)
    {
        // Implement your logic here. For now, return a random item
        if (availableItems.Count > 0)
        {
            return availableItems[Random.Range(0, availableItems.Count)];
        }
        return null;
    }

    private bool ContinueShopping()
    {
        return Random.value > 0.2f; // 80% chance to continue shopping after each purchase
    }

    void EvaluateAndSelectItems()
    {
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
                    purchasedItems.Add(new PurchasedItem { itemName = shelfItem.itemName, quantity = 1, price = shelfItem.sellingPrice, profitPerItem = profit });

                    shelfItem.UpdateUI();
                }
            }
        }
    }

    // Helper method to find a ShelfItemUI by item name
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

        // Calculate reputation change
        float reputationChange = 0f;
        int foundItemsCount = purchasedItems.Count;
        int notFoundItemsCount = desiredItems.Count - foundItemsCount;

        // For each found item, increase reputation
        reputationChange += foundItemsCount * 0.1f; // 0.1% per item found

        // Double reputation if all desired items were found and purchased
        if (foundItemsCount == desiredItems.Count)
        {
            reputationChange *= 2; // Double the reputation change
        }

        // Deduct reputation for not found items
        reputationChange -= notFoundItemsCount * 0.02f; // 0.02% per item not found

        // Communicate the change to CustomerSpawner
        CustomerSpawner customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.UpdateReputation(reputationChange);
        }

        UpdateUI();
    }

    string GenerateRandomName()
    {
        string initial = firstInitials[Random.Range(0, firstInitials.Length)];
        string surname = surnames[Random.Range(0, surnames.Length)];
        return initial + ". " + surname;
    }


    float GetItemPrice(string itemName)
    {
        return shelfManager.GetItemPrice(itemName); // Ensure this method exists in ShelfManager
    }

    void GoToTill()
    {
        TillManager tillManager = FindObjectOfType<TillManager>();
        if (tillManager != null)
        {
            tillManager.AddCustomerToQueue(this);
            // Note: Do not destroy here. Let TillManager handle when it's time to destroy.
        }
    }

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







    private bool IsItemDesired(string itemName)
    {
        return desiredItems.Contains(itemName);
    }

    private bool IsPriceAcceptable(float price)
    {
        return price <= budget; // Simple check against the budget
    }



    IEnumerator RemoveCustomerIfNoPurchase()
    {
        yield return new WaitForSeconds(4f); // Wait before checking for next action

        if (itemsInBasket == 0)
        {
            Debug.Log($"Removing customer: {customerName} due to 0 items in basket.");
            InformationBar.Instance.DisplayMessage($"{customerName} left without purchasing.");
            Destroy(gameObject); // Remove this customer object if they haven't selected any items
        }
        else
        {
            // If the customer has items in their basket, move them to the till instead of removing them
            GoToTill();
            //Destroy(gameObject);
        }
    }



    void Start()
    {
        StartCoroutine(StartShoppingRoutine());
        customerName = GenerateRandomName();
        itemsInBasket = 0;
        UpdateUI();
    }

    IEnumerator StartShoppingRoutine()
    {
        yield return new WaitForSeconds(2f);
        EvaluateAndSelectItems();
        GenerateFeedback();
        StartCoroutine(RemoveCustomerIfNoPurchase());
    }

    void UpdateUI()
    {
        nameText.text = customerName;
        itemsText.text = itemsInBasket.ToString();
        feedbackText.text = feedback; // This will now include the color tags
    }


    public void AddToBasket(int amount)
    {
        itemsInBasket += amount;
        UpdateUI();
    }

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

    public float GetItemCost(string itemName)
    {
        // Implementation depends on how ShelfManager stores item costs
        return shelfManager.GetItemCost(itemName);
    }

    public List<PurchasedItem> GetPurchasedItems()
    {
        return purchasedItems;
    }

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

    public void ProvideFeedback(string message)
    {
        feedback = message;
        UpdateUI();
    }


    [System.Serializable]
    public class PurchasedItem
    {
        public string itemName;
        public float price;
        public float profitPerItem;
        public int quantity;
    }

}
