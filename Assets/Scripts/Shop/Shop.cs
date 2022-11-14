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


    [Header("I will only buy these type of items from you.")]
    public ItemType SellingType;

    public ShopStock[] Stocks = new ShopStock[0];

    public SpeechData GreetSpeech = new SpeechData();


    //Stock = Item listing, int = Amount of times it has been added to the cart
    private Dictionary<ShopStock, int> m_shoppingCart = new Dictionary<ShopStock, int>();

    private bool m_buying = false;

    public void Update()
    {
    }

    public override void OnPlayerInteract()
    {
        //I guess we can't really do anything with this guy!
        if (!AllowBuying && !AllowSelling)
            return;

        //Start a speech, then present a choice prompt after the speech finishes through a callback.
        //Yes, this is semi-hardcoded, but i didn't see any need to make this part dynamic considering the
        //scope of this interview's goals. I would develop a more elegant, dynamic solution as needed in a real project.

        UIManager.DoSpeech(GreetSpeech.Speech, GreetSpeech.Speaker, GreetSpeech.Speed,
            delegate
            {

                //We can both buy and sell from this salesman, present a dialogue choice
                if (AllowBuying && AllowSelling)
                {
                    //Open shop in different modes based on the return value.
                    UIManager.DoSpeechChoice("I am here to...", new string[] { "Buy", "Sell", "Nevermind" },
                        delegate (int choice)
                        {
                            if (choice < 2)
                            {
                                bool buy = choice == 0;
                                OpenShop(buy);
                            }
                        });
                }
                else
                {
                    OpenShop(AllowBuying);
                }
            });
    }

    public bool IsBuying()
    {
        return m_buying;
    }

    public void CloseShop()
    {
        RootScript.PlayerBusy = false;

        m_shoppingCart.Clear();
    }

    public void OpenShop(bool buying)
    {
        RootScript.PlayerBusy = true;
        UIManager.InitializeShop(this, buying);
    }


    /// <summary>
    /// Returns the shopping cart on success, null on failure, used UNIVERSALLY on buying and selling
    /// </summary>
    /// <returns></returns>
    public Dictionary<ShopStock, int> TryCheckout()
    {
        int totalGold = GetTotalCost();

        if (m_buying)
        {
            if (CharacterPlayer.Instance.Inventory.Gold < totalGold)
                return null;
        }
        else
        {
            if (LinkedCharacter.Inventory.Gold < totalGold)
                return null;
        }

        if (m_buying)
        {
            CharacterPlayer.Instance.Inventory.Gold -= totalGold;

            if (LinkedCharacter != null)
                LinkedCharacter.Inventory.Gold += totalGold;
        }
        else
        {
            CharacterPlayer.Instance.Inventory.Gold += totalGold;

            if (LinkedCharacter != null)
                LinkedCharacter.Inventory.Gold -= totalGold;

        }

        //Duplicate the cart, this is theo ne that will get sent
        Dictionary<ShopStock, int> cartDupl = new Dictionary<ShopStock, int>(m_shoppingCart);
        m_shoppingCart.Clear();

        return cartDupl;
    }

    public int GetTotalCost()
    {
        int totalGold = 0;

        if (m_buying)
        {
            foreach (var kv in m_shoppingCart)
                totalGold += kv.Key.Item.BuyPrice * kv.Value;
        }
        else
        {
            foreach (var kv in m_shoppingCart)
                totalGold += kv.Key.Item.SellPrice * kv.Value;
        }

        return totalGold;
    }

    /// <summary>
    /// Returns if item was successfully added to cart, used UNIVERSALLY for buying and selling
    /// </summary>
    public bool TryAddToCart(ShopStock shopStock)
    {
        if (shopStock.Stock == 0)
            return false;

        if (shopStock.Stock > -1 && m_shoppingCart.ContainsKey(shopStock))
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

    public bool RemoveFromCart(ShopStock shopStock)
    {
        if (!m_shoppingCart.ContainsKey(shopStock))
            return false;

        if (m_shoppingCart[shopStock]-- <= 0)
            m_shoppingCart.Remove(shopStock);

        if (!m_shoppingCart.ContainsKey(shopStock))
            return false;

        return true;
    }
}
