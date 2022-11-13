using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour
{
    //UI objects 
    private RectTransform m_playerStandPoint;
    private RectTransform m_itemsPanel;
    private RectTransform m_itemTemplate;
    private Button m_genericButton;
    private Button m_outfitButton;

    private List<Image> m_playerPreviewImages = new List<Image>();
    private List<InventoryUIEntry> m_inventoryUIEntries = new List<InventoryUIEntry>();

    private void Awake()
    {
        m_playerStandPoint = (RectTransform)transform.Find("PlayerBG/PlayerCard/PlayerStandPoint");
        m_itemsPanel = (RectTransform)transform.Find("ItemsPanel");
        m_itemTemplate = (RectTransform)m_itemsPanel.Find("ItemTemplate");
        m_genericButton = transform.Find("GenericButton").GetComponent<Button>();
        m_outfitButton = transform.Find("OutfitButton").GetComponent<Button>();

        m_genericButton.onClick.AddListener(delegate { FilterInventory(ItemType.Generic); });
        m_outfitButton.onClick.AddListener(delegate { FilterInventory(ItemType.Outfit); });

        m_itemTemplate.gameObject.SetActive(false);

        GenerateImages();
    }

    private void Update()
    {
        RootScript.PlayerBusy = true;
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

            RectTransform root = Instantiate(m_itemTemplate);
            Image border = root.Find("Frame").GetComponent<Image>();
            Image icon = border.transform.Find("Icon").GetComponent<Image>();

            entry.Root = root;
            entry.Border = border;
            entry.Icon = icon;
            entry.Item = item;

            root.SetParent(m_itemsPanel, false);
            root.gameObject.SetActive(true);
            icon.sprite = item.Item.Icon;

            m_inventoryUIEntries.Add(entry);
        }
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

        gameObject.SetActive(false);
    }
}
