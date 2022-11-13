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


    /// <summary>
    /// Remove item X amount of items
    /// </summary>
    /// <param name="itemDef"></param>
    /// <param name="count"></param>
    public void RemoveItem(ItemDefinition itemDef, int count = 1)
    {
        if (count <= 0)
            return;

        InventoryItem[] items = GetItems(itemDef);

        int removedCount = 0;

        foreach(InventoryItem item in items)
        {
            int itemCount = item.Count;

            for (int i = 0; i < itemCount; i++)
            {
                item.Count--;
                removedCount++;

                if (item.Count <= 0)
                    Items.Remove(item);

                if (removedCount == count)
                    break;
            }
        }

    }

    public InventoryItem GetItem(ItemDefinition item)
    {
        return Items.FirstOrDefault(x => x.Item == item);
    }

    public InventoryItem[] GetItems(ItemDefinition item)
    {
        return Items.Where(x => x.Item == item).ToArray();
    }


    public InventoryItem[] GetItemsOfType(ItemType itemType)
    {
        return Items.Where(x => x.Item.Type.HasFlag(itemType)).ToArray();
    }
    public bool HasItem(ItemDefinition item)
    {
        return GetItem(item) != null;
    }
}
