using System;
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
    private SpeechUI m_speechUI = null;
    private SpeechChoiceUI m_speechChoiceUI = null;
    private PlayerInventoryUI m_playerInventoryUI = null;

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
        m_speechUI = m_uiRoot.Find("SpeechUI").GetComponent<SpeechUI>();
        m_speechChoiceUI = m_uiRoot.Find("SpeechChoiceUI").GetComponent<SpeechChoiceUI>();
        m_playerInventoryUI = m_uiRoot.Find("PlayerInventory").GetComponent<PlayerInventoryUI>();

        m_shopUI.gameObject.SetActive(false);
        m_speechUI.gameObject.SetActive(false);
        m_speechChoiceUI.gameObject.SetActive(false);
        m_playerInventoryUI.gameObject.SetActive(false);

       // m_speechChoiceUI.Initialize(new string[] { "Buy", "Sell" }, null);
    }

    public static void InitializeShop(Shop shop)
    {
        Instance.m_shopUI.ProcessShopUI(shop);
    }

    /// <summary>
    /// Do speech, just one line.
    /// </summary>
    public static void DoSpeech(string speech, string speaker, Action finishedCallback = null)
    {
        DoSpeech(new string[] { speech }, speaker, finishedCallback);
    }

    /// <summary>
    /// Do speech, one speaker only.
    /// </summary>
    public static void DoSpeech(string[] speech, string speaker, Action finishedCallback = null)
    {
        string[] speakerArr = new string[speech.Length];
        Array.Fill(speakerArr, speaker);

        DoSpeech(speech, speakerArr, finishedCallback);
    }

    /// <summary>
    /// Do speech, with multiple speakers, speaker array must be same length as speech.
    /// </summary>
    public static void DoSpeech(string[] speech, string[] speaker, Action finishedCallback = null)
    {
        Instance.m_speechUI.Initialize(speech, speaker, finishedCallback);
    }

    /// <summary>
    /// Present a choice to the player.
    /// </summary>
    public static void DoSpeechChoice(string speech, string[] choices, Action<int> m_finishedCallback)
    {
        Instance.m_speechChoiceUI.Initialize(speech, choices, m_finishedCallback);
    }

    public static void ShowPlayerInventory()
    {
        Instance.m_playerInventoryUI.OpenInventory();
    }
}
