using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopStock
{
    public ItemDefinition Item;
    [Header("-1 = Infinite")]
    public int Stock = -1;
}
