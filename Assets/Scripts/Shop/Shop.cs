using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : InteractableEntity
{
    [Header("Character to link this shop to, not required.")]
    public BaseCharacter LinkedCharacter = null;

    public bool AllowBuying = true;
    public bool AllowSelling = true;

    public ShopStock[] Stocks = new ShopStock[0];

    //Stock = Item listing, int = Amount of times it has been added to the cart
    private Dictionary<ShopStock, int> m_shoppingCart = new Dictionary<ShopStock, int>();

    public void Update()
    {
    }

    public override void OnPlayerInteract()
    {
        //Start a speech, then present a choice prompt after the speech finishes through a callback.
        //Yes, this is hardcoded, but i didn't see any need to make this part dynamic considering the
        //scope of this interview's goals.
        UIManager.DoSpeech("Hey! How can i help you today?", "Salesman", 
            delegate 
            {
                //Open shop in different modes based on the return value.
                UIManager.DoSpeechChoice("I am here to...", new string[] { "Buy", "Sell", "Nevermind" }, 
                    delegate(int choice)
                    {
                        if (choice < 2)
                        {
                            bool buy = choice == 0;
                            OpenShop();
                        }
                    });
            });
    }

    public void CloseShop()
    {
        RootScript.PlayerBusy = false;

        m_shoppingCart.Clear();
    }

    public void OpenShop()
    {
        RootScript.PlayerBusy = true;
        UIManager.InitializeShop(this);
    }

    
    /// <summary>
    /// Returns the shopping cart on success, null on failure
    /// </summary>
    /// <returns></returns>
    public Dictionary<ShopStock, int> TryBuy()
    {
        int totalGold = GetTotalCost();

        if (CharacterPlayer.Instance.Inventory.Gold < totalGold)
            return null;

        CharacterPlayer.Instance.Inventory.Gold -= totalGold;

        if (LinkedCharacter != null)
            LinkedCharacter.Inventory.Gold += totalGold;

        //Duplicate the cart, this is theo ne that will get sent
        Dictionary<ShopStock, int> cartDupl = new Dictionary<ShopStock, int>(m_shoppingCart);
        m_shoppingCart.Clear();

        return cartDupl;
    }

    public int GetTotalCost()
    {
        int totalGold = 0;

        foreach (var kv in m_shoppingCart)
            totalGold += kv.Key.Item.BuyPrice * kv.Value;

        return totalGold;
    }

    /// <summary>
    /// Returns if item was successfully added to cart
    /// </summary>
    public bool TryAddToCart(ShopStock shopStock)
    {
        if (shopStock.Stock == 0)
            return false;

        if(shopStock.Stock > -1 && m_shoppingCart.ContainsKey(shopStock))
        {
            if (m_shoppingCart[shopStock] + 1 > shopStock.Stock)
                return false;
        }

        if (!m_shoppingCart.ContainsKey(shopStock))
            m_shoppingCart.Add(shopStock, 1);
        else
            m_shoppingCart[shopStock] += 1;

        return true;
    }

    public void RemoveFromCart(ShopStock shopStock)
    {
        if (!m_shoppingCart.ContainsKey(shopStock))
            return;

        if (m_shoppingCart[shopStock]-- <= 0)
            m_shoppingCart.Remove(shopStock);
    }
}
