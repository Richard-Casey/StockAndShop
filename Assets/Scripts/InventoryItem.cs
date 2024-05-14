using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an inventory item in the shop.
/// </summary>
[System.Serializable]
public class InventoryItem
{
    /// <summary>
    /// The base demand for the item.
    /// </summary>
    public float baseDemand;

    /// <summary>
    /// The cost price of the item.
    /// </summary>
    public float cost;

    /// <summary>
    /// The current demand for the item.
    /// </summary>
    public float demand;

    /// <summary>
    /// The image of the item.
    /// </summary>
    public Sprite itemImage;

    /// <summary>
    /// The name of the item.
    /// </summary>
    public string itemName;

    /// <summary>
    /// The quantity of the item in stock.
    /// </summary>
    public int quantity;

    /// <summary>
    /// The selling price of the item.
    /// </summary>
    public float sellingPrice;

    /// <summary>
    /// The quantity of the item available for sale.
    /// </summary>
    public int sellQuantity;

    /// <summary>
    /// Indicates whether the item is selected for selling.
    /// </summary>
    public bool isSelectedForSelling { get; set; }

}
