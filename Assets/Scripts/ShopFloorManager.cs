using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the items available on the shop floor.
/// </summary>
public class ShopFloorManager : MonoBehaviour
{
    /// <summary>
    /// The list of inventory items available on the shop floor.
    /// </summary>
    public List<InventoryItem> shopFloorItems = new List<InventoryItem>();

    /// <summary>
    /// Adds an item to the shop floor inventory.
    /// </summary>
    /// <param name="itemToAdd">The inventory item to add to the shop floor.</param>
    public void AddItemToShopFloor(InventoryItem itemToAdd)
    {
        shopFloorItems.Add(itemToAdd);
    }
}
