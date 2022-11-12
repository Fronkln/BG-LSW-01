using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Inventory
{
    public List<InventoryItem> Items = new List<InventoryItem>();
    public int Gold = 0;


    public void AddItem(ItemDefinition item, int count, bool stack = false)
    {
        InventoryItem preExistingItem = GetItem(item);

        if (preExistingItem == null || !stack)
            Items.Add(new InventoryItem(item, count));
        else
            preExistingItem.Count += count;
    }

    public InventoryItem GetItem(ItemDefinition item)
    {
        return Items.FirstOrDefault(x => x.Item == item);
    }

    public bool HasItem(ItemDefinition item)
    {
        return GetItem(item) != null;
    }
}
