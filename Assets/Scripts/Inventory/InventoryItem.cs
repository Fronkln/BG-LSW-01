using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemDefinition Item;
    public int Count;

    public InventoryItem(ItemDefinition item, int count)
    {
        Item = item;
        Count = count;
    }
}
