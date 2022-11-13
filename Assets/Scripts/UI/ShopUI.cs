using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class ShopUI : MonoBehaviour
{
    //UI Objects
    [HideInInspector] public RectTransform DescriptionPanel;
    [HideInInspector] public TextMeshProUGUI DescriptionText;
    [HideInInspector] public Image ItemIcon;

    [HideInInspector] public RectTransform ItemsPanel;
    [HideInInspector] public RectTransform ItemTemplate;

    [HideInInspector] public RectTransform GoldsRoot;
    [HideInInspector] public TextMeshProUGUI PlayerGold;
    [HideInInspector] public TextMeshProUGUI SellerGold;
    [HideInInspector] public TextMeshProUGUI TotalGold;

    //Sounds
    [SerializeField] private AudioClip m_sfxShopCartAdd;
    [SerializeField] private AudioClip m_sfxShopFail;

    //Colors
    private static readonly Color32 m_unselectedColor = new Color32(94, 94, 91, 255);
    private static readonly Color32 m_highlightedColor = new Color32(144, 130, 0, 255);

    private Shop m_currentShop = null;
    private List<ShopUIEntry> m_createdItems = new List<ShopUIEntry>();
    private int m_currentSelectedItem = 0;

    private AudioSource m_soundPlayer = null;

    private void Awake()
    {
        DescriptionPanel = (RectTransform)transform.Find("DescriptionPanel");
        DescriptionText = DescriptionPanel.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        ItemIcon = DescriptionPanel.Find("IconBorder/Icon").GetComponent<Image>();
        ItemsPanel = (RectTransform)transform.Find("ItemsPanelBorder/ItemsPanel");
        ItemTemplate = (RectTransform)ItemsPanel.Find("ItemTemplate");

        ItemTemplate.gameObject.SetActive(false);

        GoldsRoot = (RectTransform)ItemsPanel.Find("Golds");
        PlayerGold = GoldsRoot.Find("Frame/PlayerGold").GetComponent<TextMeshProUGUI>();
        SellerGold = GoldsRoot.Find("Frame/SellerGold").GetComponent<TextMeshProUGUI>();
        TotalGold = GoldsRoot.Find("Frame/TotalGold").GetComponent<TextMeshProUGUI>();

        m_soundPlayer = GetComponent<AudioSource>();
    }

    public void ProcessShopUI(Shop shop)
    {
        ClearShopUI();
        m_currentShop = shop;

        gameObject.SetActive(true);

        for (int i = 0; i < shop.Stocks.Length; i++)
            AddItemToListing(shop.Stocks[i]);

        OnSelectedItemChanged(0, 0);
        UpdateValues();
    }

    public void AddItemToListing(ShopStock stock)
    {
        RectTransform newItemTemplate = Instantiate(ItemTemplate);
        newItemTemplate.transform.parent = ItemsPanel;

        Image border = newItemTemplate.GetComponent<Image>();
        Image itemIconImg = newItemTemplate.Find("ItemFg/ItemIcon").GetComponent<Image>();

        ShopUIEntry uiEntry = new ShopUIEntry();
        uiEntry.Root = newItemTemplate;
        uiEntry.Border = border;
        uiEntry.Icon = itemIconImg;
        uiEntry.Stock = stock;

        uiEntry.Border.color = m_unselectedColor;
        uiEntry.Icon.sprite = stock.Item.Icon;
        uiEntry.Root.gameObject.SetActive(true);

        m_createdItems.Add(uiEntry);
    }

    private void ClearShopUI()
    {
        foreach (ShopUIEntry entry in m_createdItems)
            Destroy(entry.Root.gameObject);

        m_currentSelectedItem = 0;
        m_createdItems.Clear();
    }

    private void Update()
    {
        if (m_currentShop == null)
            return;

        RootScript.PlayerBusy = true;

        int selectedItem = m_currentSelectedItem;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            selectedItem -= 1;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            selectedItem += 1;

        if (selectedItem < 0)
            selectedItem = m_currentShop.Stocks.Length - 1;
        else if (selectedItem >= m_currentShop.Stocks.Length)
            selectedItem = 0;

        if (selectedItem != m_currentSelectedItem)
        {
            OnSelectedItemChanged(m_currentSelectedItem, selectedItem);
            m_currentSelectedItem = selectedItem;
        }

        //Add/remove item from cart
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            bool success = m_currentShop.TryAddToCart(GetSelectedStock());

            if (success)
                m_soundPlayer.PlayOneShot(m_sfxShopCartAdd);
            else
                OnShopFailure();

            UpdateValues();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_currentShop.RemoveFromCart(GetSelectedStock());
            UpdateValues();
        }

        //Buying
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Dictionary<ShopStock, int> cart = m_currentShop.TryBuy();

            if (cart == null)
                OnShopFailure();
            else
                OnShopSuccess(cart);
        }
    }


    public void OnShopSuccess(Dictionary<ShopStock, int> cart)
    {
        m_soundPlayer.PlayOneShot(m_sfxShopCartAdd);

        foreach (var kv in cart)
            CharacterPlayer.Instance.Inventory.AddItem(kv.Key.Item, kv.Value, true);

        UpdateValues();

        Invoke("CloseShop", 1);
    }
    private void OnShopFailure()
    {
        m_soundPlayer.Stop();
        m_soundPlayer.PlayOneShot(m_sfxShopFail);
    }

    private void CloseShop()
    {
        gameObject.SetActive(false);
        RootScript.PlayerBusy = false;
    }

    public ShopStock GetSelectedStock()
    {
        return m_createdItems[m_currentSelectedItem].Stock;
    }

    private void UpdateValues()
    {
        PlayerGold.text = "YOUR GOLD: " + CharacterPlayer.Instance.Inventory.Gold;

        if (m_currentShop.LinkedCharacter == null)
            SellerGold.text = "SELLER GOLD: 0";
        else
            SellerGold.text = "SELLER GOLD: " + m_currentShop.LinkedCharacter.Inventory.Gold;

        TotalGold.text = "TOTAL: " + m_currentShop.GetTotalCost();
    }

    private void OnSelectedItemChanged(int oldIdx, int newIdx)
    {
        ShopUIEntry m_oldSelected = m_createdItems[oldIdx];
        ShopUIEntry m_newSelected = m_createdItems[newIdx];

        m_oldSelected.Border.color = m_unselectedColor;
        m_newSelected.Border.color = m_highlightedColor;

        DescriptionText.text = m_newSelected.Stock.Item.Description;
        ItemIcon.sprite = m_newSelected.Stock.Item.Icon;
    }
}
