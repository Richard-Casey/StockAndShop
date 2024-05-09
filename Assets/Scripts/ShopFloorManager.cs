using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ShopFloorManager : MonoBehaviour
{
    /// <summary>
    /// The shop floor items
    /// </summary>
    public List<InventoryItem> shopFloorItems = new List<InventoryItem>();

    /// <summary>
    /// Adds the item to shop floor.
    /// </summary>
    /// <param name="itemToAdd">The item to add.</param>
    public void AddItemToShopFloor(InventoryItem itemToAdd)
    {
        shopFloorItems.Add(itemToAdd);
    }
}
