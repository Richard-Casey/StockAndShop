# Stock & Shop

Stock & Shop is a shop simulator focusing on economy and reputation. Players manage a shop by purchasing stock, setting prices, and serving customers. The game's goal is to manage the shop effectively, ensuring customer satisfaction and maintaining a good reputation.

## Table of Contents
- [Getting Started](#getting-started)
- [Game Overview](#game-overview)
  - [Wholesale Menu](#wholesale-menu)
  - [Inventory Management](#inventory-management)
  - [Pricing Items](#pricing-items)
  - [Opening the Shop](#opening-the-shop)
  - [Customer Interaction](#customer-interaction)
  - [Reputation System](#reputation-system)
- [Scripts](#scripts)
- [License](#license)
- [Contact](#contact)

  ## Getting Started

1. **Clone the repository:**
   git clone https://github.com/Richard-Casey/StockAndShop.git

2. **Open the project in Unity:**
   - Open Unity Hub.
   - Click on the `Add` button and navigate to the cloned project folder.
   - Open the project.

3. **Play the game:**
   - Press the `Play` button in the Unity editor to start the game.
  
## Game Overview

### Wholesale Menu
- **Starting Funds:** You start the game with £50.
- **Buying Stock:** Use the wholesale menu to purchase items for your shop. Once purchased, items will appear in your inventory.

### Inventory Management
- **View Inventory:** Access the inventory/stock screen to view items.
- **Set Prices:** Set the price for each item. A color-coded bar will indicate if the price is set appropriately:
  - Green: Cheap
  - Yellow: About right
  - Red: Priced too high

### Pricing Items
- **Price Indicators:** Ensure prices are set to maximise sales while maintaining customer satisfaction.

### Opening the Shop
- **Start Day Cycle:** Press the "Open Shop" button to start the day cycle, allowing customers to enter and shop from 9 AM to 5 PM.
- **Repeat:** Pressing the "Open Shop" button again will start a new day.

### Customer Interaction
- **Footfall:** The number of customers is determined by the shop's reputation.
- **Satisfaction:** Customer satisfaction depends on finding desired items and reasonable pricing.

### Reputation System
- **Positive Reputation:** Achieved by satisfying customer needs with appropriately priced items.
- **Negative Reputation:** Results from customers being unable to find items or finding them too expensive.

## Scripts

Below are the main scripts used in the game:

- **BuyItemHandler.cs:** Handles the purchasing of items.
- **CashDisplay.cs:** Manages the display of the shop's cash balance.
- **Customer.cs:** Controls customer behavior and interactions.
- **CustomerSpawner.cs:** Spawns customers into the shop.
- **DailySummaryManager.cs:** Manages the summary of daily activities.
- **DayCycle.cs:** Handles the day cycle and shop opening/closing times.
- **DynamicContentSizeForOneColumn.cs:** Adjusts UI content size for a single column layout.
- **DynamicContentSizeForTwoColumns.cs:** Adjusts UI content size for a two-column layout.
- **GameManagerQuit.cs:** Manages quitting the game.
- **InformationBar.cs:** Displays various information bars in the UI.
- **InventoryItem.cs:** Represents an item in the inventory.
- **InventoryItemUI.cs:** Manages the UI for inventory items.
- **InventoryManager.cs:** Manages the inventory and stock levels.
- **NavigationController.cs:** Handles navigation between different UI screens.
- **OverallSummaryManager.cs:** Manages the overall summary of the game.
- **RecieptGenerator.cs:** Generates receipts for purchased items.
- **ShelfItemUI.cs:** Manages the UI for items placed on shelves.
- **ShelfManager.cs:** Manages items on the shop floor shelves.
- **ShopFloorManager.cs:** Manages the overall shop floor operations.
- **StartAtTop.cs:** Ensures UI starts at the top of the scroll view.
- **SummaryPrefabScript.cs:** Manages summary prefabs in the UI.
- **SyncPositionWithButton.cs:** Syncs UI positions with buttons.
- **TillManager.cs:** Manages the till and checkout process.
- **WholesaleItemUI.cs:** Manages the UI for wholesale items.
- **WholesaleManager.cs:** Manages the wholesale purchasing process.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any questions or further information, please contact Richard Casey at Me@Richard-Casey.co.uk.

