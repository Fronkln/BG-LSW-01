using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LSW 01/Item/Generic Item")]
public class ItemDefinition : ScriptableObject
{
    public string Name = "Item";
    public string Description = "Whoa, it's an item!";

    public int BuyPrice = 10;
    public int SellPrice = 10;

    public virtual ItemType Type => ItemType.Generic;

    public Sprite Icon = null;


    /// <summary>
    /// Returns if the item was successfully used.
    /// </summary>
    public virtual bool OnUseItem(BaseCharacter usingCharacter)
    {
        return false;
    }
}

public enum ItemType
{
    Generic,
    Outfit,
}
