using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SpeechChoiceUI : MonoBehaviour
{
    //UI Objects
    private TextMeshProUGUI m_optionTemplate;
    private TextMeshProUGUI m_speech;
    private TextMeshProUGUI m_selectedOptionIcon;
    private RectTransform m_choiceGrid;

    private AudioSource m_soundEmitter;

    //Sounds
    [SerializeField] private AudioClip m_selectedAudioclip;
    [SerializeField] private AudioClip m_confirmAudioclip;

    //Colors
    private readonly Color32 m_selectedColor = new Color32(204, 204, 0, 255);

    //Misc variables
    private int m_selectedOption = 0;
    private List<TextMeshProUGUI> m_createdOptions = new List<TextMeshProUGUI>();

    private Action<int> m_chosenOptionCallback = null;

    private bool m_active = false;
    private bool m_choiceMade = false;

    void Awake()
    {
        m_choiceGrid = (RectTransform)transform.Find("SpeechInside/ChoiceGrid");
        m_selectedOptionIcon = transform.Find("SpeechInside/Choice").GetComponent<TextMeshProUGUI>();
        m_speech = transform.Find("SpeechInside/Speech").GetComponent<TextMeshProUGUI>();
        m_optionTemplate = m_choiceGrid.Find("OptionTemplate").GetComponent<TextMeshProUGUI>();
        m_soundEmitter = gameObject.GetComponent<AudioSource>();

        m_optionTemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_active || m_choiceMade)
            return;

        RootScript.PlayerBusy = true;

        int selectedOption = m_selectedOption;

        //Choosing
        if (Input.GetKeyDown(KeyCode.W))
            selectedOption -= 1;
        else if (Input.GetKeyDown(KeyCode.S))
            selectedOption += 1;

        if (selectedOption >= m_createdOptions.Count)
            selectedOption = 0;
        else if (selectedOption < 0)
            selectedOption = m_createdOptions.Count - 1;

        if (selectedOption != m_selectedOption)
        {
            m_selectedOption = selectedOption;
            OnOptionChanged();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnOptionChosen();
        }
    }

    //Clear generated UI
    private void Clear()
    {
        m_selectedOption = 0;
        m_chosenOptionCallback = null;
        m_selectedOptionIcon.color = Color.white;

        foreach (TextMeshProUGUI transform in m_createdOptions)
            Destroy(transform.gameObject);

        m_createdOptions.Clear();
    }

    /// <summary>
    /// Generate UI and activate a choice prompt.
    /// </summary>
    public void Initialize(string speech, string[] options, Action<int> onOptionChosen)
    {
        Clear();

        m_active = true;
        m_chosenOptionCallback = onOptionChosen;

        m_speech.text = speech;

        for (int i = 0; i < options.Length; i++)
        {
            TextMeshProUGUI newChoice = Instantiate(m_optionTemplate);
            newChoice.transform.parent = m_choiceGrid;
            newChoice.text = options[i];
            newChoice.gameObject.SetActive(true);

            m_createdOptions.Add(newChoice);
        }

        gameObject.SetActive(true);
    }

    private void OnOptionChanged()
    {
        m_soundEmitter.PlayOneShot(m_selectedAudioclip);

        m_selectedOptionIcon.transform.position = new Vector3
            (m_selectedOptionIcon.transform.position.x,
            m_createdOptions[m_selectedOption].transform.position.y,
            m_selectedOptionIcon.transform.position.z);
    }

    private void OnOptionChosen()
    {
        m_choiceMade = true;
        m_createdOptions[m_selectedOption].color = m_selectedColor;
        m_selectedOptionIcon.color = m_selectedColor;
        m_soundEmitter.PlayOneShot(m_confirmAudioclip);
        Invoke("Finish", 1);
    }

    private void Finish()
    {
        //Reset position
        m_selectedOptionIcon.transform.position = new Vector3
            (m_selectedOptionIcon.transform.position.x,
            m_createdOptions[0].transform.position.y,
            m_selectedOptionIcon.transform.position.z);

        m_active = false;
        m_choiceMade = false;
        gameObject.SetActive(false);
        m_chosenOptionCallback?.Invoke(m_selectedOption);
        RootScript.PlayerBusy = false;
    }
}
