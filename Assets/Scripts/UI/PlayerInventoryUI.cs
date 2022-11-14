using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventoryUI : MonoBehaviour
{
    //UI objects 
    private RectTransform m_playerStandPoint;
    private RectTransform m_itemsPanel;
    private RectTransform m_itemTemplate;
    private Button m_genericButton;
    private Button m_outfitButton;
    private AudioSource m_soundEmitter;

    //Sounds
    [SerializeField] private AudioClip m_sfxSelectionChanged;
    [SerializeField] private AudioClip m_sfxWearOutfit;

    //Colors
    private readonly Color32 m_unselectedColor = new Color32(255, 255, 255, 255);
    private readonly Color32 m_selectedColor = new Color32(61, 255, 0, 255);

    //Variables
    private List<Image> m_playerPreviewImages = new List<Image>();
    private List<InventoryUIEntry> m_inventoryUIEntries = new List<InventoryUIEntry>();

    private int m_selectedItem = 0;

    private void Awake()
    {
        m_playerStandPoint = (RectTransform)transform.Find("PlayerBG/PlayerCard/PlayerStandPoint");
        m_itemsPanel = (RectTransform)transform.Find("ItemsPanel");
        m_itemTemplate = (RectTransform)m_itemsPanel.Find("ItemTemplate");
        m_genericButton = transform.Find("GenericButton").GetComponent<Button>();
        m_outfitButton = transform.Find("OutfitButton").GetComponent<Button>();
        m_soundEmitter = transform.GetComponent<AudioSource>();

        //Button events
        m_genericButton.onClick.AddListener(delegate { FilterInventory(ItemType.Generic); });
        m_outfitButton.onClick.AddListener(delegate { FilterInventory(ItemType.Outfit); });

        m_itemTemplate.gameObject.SetActive(false);

        GenerateImages();
    }

    public void OpenInventory()
    {
        UpdatePlayerPreview();
        FilterInventory(ItemType.Generic);

        gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        UpdatePlayerPreview();
        FilterInventory(ItemType.Generic);

        RootScript.PlayerBusy = false;
        gameObject.SetActive(false);
    }

    private void OnSelectedItemChanged(int oldIdx, int newIdx)
    {
        InventoryUIEntry oldEntry = m_inventoryUIEntries[oldIdx];
        InventoryUIEntry newEntry = m_inventoryUIEntries[newIdx];

        oldEntry.Border.color = m_unselectedColor;
        newEntry.Border.color = m_selectedColor;

        m_soundEmitter.PlayOneShot(m_sfxSelectionChanged);
    }

    private void Update()
    {
        RootScript.PlayerBusy = true;

        int selectedItem = m_selectedItem;

        if (Input.GetKeyDown(KeyCode.D))
            selectedItem += 1;
        if (Input.GetKeyDown(KeyCode.A))
            selectedItem -= 1;

        if (selectedItem < 0)
            selectedItem = m_inventoryUIEntries.Count - 1;
        if (selectedItem >= m_inventoryUIEntries.Count)
            selectedItem = 0;

        if(selectedItem != m_selectedItem)
        {
            OnSelectedItemChanged(m_selectedItem, selectedItem);
            m_selectedItem = selectedItem;
        }

        //Use item
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InventoryItem invItem = GetSelectedInventoryItem();
            bool used = invItem.Item.OnUseItem(CharacterPlayer.Instance);

            if (used)
                if (invItem.Item.Type == ItemType.Outfit)
                {
                    UpdatePlayerPreview();
                    m_soundEmitter.PlayOneShot(m_sfxWearOutfit);
                }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
            CloseInventory();
    }


    private void GenerateImages()
    {
        for (int i = 0; i < CharacterAppearance.BodypartCount; i++)
        {
            GameObject part = new GameObject("Bodypart");
            Image img = part.AddComponent<Image>();
            img.rectTransform.sizeDelta = new Vector2(272.16f, 436f);

            part.transform.SetParent(m_playerStandPoint, false);
            m_playerPreviewImages.Add(img);
        }
    }

    private void ClearInventory()
    {
        foreach (InventoryUIEntry entry in m_inventoryUIEntries)
            Destroy(entry.Root.gameObject);

        m_inventoryUIEntries.Clear();
    }

    private void FilterInventory(ItemType itemFilter)
    {
        ClearInventory();

        IEnumerable<InventoryItem> items = CharacterPlayer.Instance.Inventory.Items.Where(x => x.Item.Type == itemFilter);

        foreach(InventoryItem item in items)
        {
            InventoryUIEntry entry = new InventoryUIEntry();

            //Locate the UI components
            RectTransform root = Instantiate(m_itemTemplate);
            Image border = root.Find("Frame").GetComponent<Image>();
            Image icon = border.transform.Find("Icon").GetComponent<Image>();
            RectTransform countDisplayRoot = (RectTransform)border.transform.Find("CountRoot");
            TextMeshProUGUI countDisplay = countDisplayRoot.Find("Count").GetComponent<TextMeshProUGUI>();

            entry.Root = root;
            entry.Border = border;
            entry.Icon = icon;
            entry.Item = item;
            entry.CountDisplay = countDisplay;
            entry.CountDisplayRoot = countDisplayRoot;

            root.SetParent(m_itemsPanel, false);
            root.gameObject.SetActive(true);
            icon.sprite = item.Item.Icon;

            countDisplay.text = item.Count.ToString();

            if (item.Count <= 1)
                countDisplayRoot.gameObject.SetActive(false);
            

            m_inventoryUIEntries.Add(entry);
        }

        m_selectedItem = 0;
        OnSelectedItemChanged(0, 0);
    }

    private InventoryItem GetSelectedInventoryItem()
    {
        return m_inventoryUIEntries[m_selectedItem].Item;
    }

    public void UpdatePlayerPreview()
    {
        Sprite[] characterSprites = CharacterPlayer.Instance.Appearance.GetTextures(CharacterDirection.Down);

        for (int i = 0; i < m_playerPreviewImages.Count; i++)
        {
            m_playerPreviewImages[i].sprite = characterSprites[i];

            if (characterSprites[i] == null)
                m_playerPreviewImages[i].enabled = false;
            else
                m_playerPreviewImages[i].enabled = true;
        }
    }
}
