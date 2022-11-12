using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("Character to link this shop to, not required.")]
    public BaseCharacter LinkedCharacter = null;

    public bool AllowBuying = true;
    public bool AllowSelling = true;

    public ShopStock[] Stocks = new ShopStock[0];

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenShop();
    }

    public void OpenShop()
    {
        if (RootScript.PlayerBusy)
            return;

        print("Shop open");

        RootScript.PlayerBusy = true;
        UIManager.InitializeShop(this);
    }
}
