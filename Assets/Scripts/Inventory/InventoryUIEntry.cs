using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIEntry
{
    public InventoryItem Item;

    public RectTransform Root;
    public Image Icon;
    public Image Border;
    public RectTransform CountDisplayRoot;
    public TextMeshProUGUI CountDisplay;
}
