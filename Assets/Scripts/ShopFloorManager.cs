using System.Collections.Generic;
using UnityEngine;

public class ShopFloorManager : MonoBehaviour
{
    public List<InventoryItem> shopFloorItems = new List<InventoryItem>();

    public void AddItemToShopFloor(InventoryItem itemToAdd)
    {
        shopFloorItems.Add(itemToAdd);
    }
}
