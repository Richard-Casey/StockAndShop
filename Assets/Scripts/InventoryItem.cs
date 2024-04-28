using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventoryItem
{
    public float baseDemand;
    public float cost;
    public float demand;
    public Sprite itemImage;
    public string itemName;
    public int quantity;
    public float sellingPrice;
    public int sellQuantity;

    public bool isSelectedForSelling { get; set; }

}
