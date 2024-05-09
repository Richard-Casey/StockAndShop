using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BuyItemHandler : MonoBehaviour
{

    /// <summary>
    /// Buys the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="cost">The cost.</param>
    /// <param name="quantity">The quantity.</param>
    public void BuyItem(InventoryItem item, float cost, int quantity)
    {
        // Find the GameManager GameObject by name
        GameObject gameManager = GameObject.Find("GameManager");

    }
}