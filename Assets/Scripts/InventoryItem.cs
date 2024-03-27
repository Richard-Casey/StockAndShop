using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public float cost;
    public float sellingPrice;
    public int quantity;
    public int sellQuantity;
    public Sprite itemImage;
    public float demand;
    public float baseDemand;
    public bool isSelectedForSelling { get; set; }

}
