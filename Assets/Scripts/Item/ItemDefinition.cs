using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LSW 01/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string Name = "Item";
    public string Description = "Whoa, it's an item!";

    public int BuyPrice = 10;
    public int SellPrice = 10;

    public ItemType Type = ItemType.Generic;

    public Sprite Icon = null;
}

public enum ItemType
{
    Generic,
    Outfit,
}
