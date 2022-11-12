using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Holds information about a item created in shop UI
[System.Serializable]
public class ShopUIEntry
{
    public RectTransform Root;
    public Image Border;
    public Image Icon;
    public ShopStock Stock;
}