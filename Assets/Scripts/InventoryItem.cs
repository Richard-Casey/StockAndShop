using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
[System.Serializable]
public class InventoryItem
{
    /// <summary>
    /// The base demand
    /// </summary>
    public float baseDemand;
    /// <summary>
    /// The cost
    /// </summary>
    public float cost;
    /// <summary>
    /// The demand
    /// </summary>
    public float demand;
    /// <summary>
    /// The item image
    /// </summary>
    public Sprite itemImage;
    /// <summary>
    /// The item name
    /// </summary>
    public string itemName;
    /// <summary>
    /// The quantity
    /// </summary>
    public int quantity;
    /// <summary>
    /// The selling price
    /// </summary>
    public float sellingPrice;
    /// <summary>
    /// The sell quantity
    /// </summary>
    public int sellQuantity;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected for selling.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected for selling; otherwise, <c>false</c>.
    /// </value>
    public bool isSelectedForSelling { get; set; }

}
