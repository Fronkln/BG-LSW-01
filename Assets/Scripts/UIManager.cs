using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //The UI that contains everything
    private RectTransform m_uiRoot = null;

    
    //Various UIs contained in UIRoot
    private ShopUI m_shopUI = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        new GameObject("UIManager").AddComponent<UIManager>();
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        m_uiRoot = Instantiate(Resources.Load<RectTransform>("GameUI"));
        m_shopUI = m_uiRoot.Find("ShopUI").GetComponent<ShopUI>();

        m_shopUI.gameObject.SetActive(false);
    }

    public static void InitializeShop(Shop shop)
    {
        Instance.m_shopUI.ProcessShopUI(shop);
    }
}
