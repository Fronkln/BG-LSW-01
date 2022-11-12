using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SpeechChoiceUI : MonoBehaviour
{
    //TODO: Naming convention
    private TextMeshProUGUI OptionTemplate;
    private TextMeshProUGUI Speech;
    private RectTransform SelectedOptionIcon;
    private RectTransform ChoiceGrid;

    private int m_selectedOption = 0;
    private List<RectTransform> m_createdOptions = new List<RectTransform>();

    private Action<int> m_chosenOptionCallback = null;

    private bool m_active = false;
    private bool m_choiceMade = false;

    void Awake()
    {
        ChoiceGrid = (RectTransform)transform.Find("SpeechInside/ChoiceGrid");
        SelectedOptionIcon = (RectTransform)transform.Find("SpeechInside/Choice");
        Speech = transform.Find("SpeechInside/Speech").GetComponent<TextMeshProUGUI>();
        OptionTemplate = ChoiceGrid.Find("OptionTemplate").GetComponent<TextMeshProUGUI>();

        OptionTemplate.gameObject.SetActive(false);
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

        foreach (RectTransform transform in m_createdOptions)
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

        Speech.text = speech;

        for (int i = 0; i < options.Length; i++)
        {
            TextMeshProUGUI newChoice = Instantiate(OptionTemplate);
            newChoice.transform.parent = ChoiceGrid;
            newChoice.text = options[i];
            newChoice.gameObject.SetActive(true);

            m_createdOptions.Add(newChoice.rectTransform);
        }

        gameObject.SetActive(true);
    }

    private void OnOptionChanged()
    {
        SelectedOptionIcon.transform.position = new Vector3
            (SelectedOptionIcon.transform.position.x,
            m_createdOptions[m_selectedOption].transform.position.y,
            SelectedOptionIcon.transform.position.z);
    }

    private void OnOptionChosen()
    {
        m_choiceMade = true;
        Invoke("Finish", 1);
    }

    private void Finish()
    {
        m_active = false;
        m_choiceMade = false;
        gameObject.SetActive(false);
        m_chosenOptionCallback?.Invoke(m_selectedOption);
        RootScript.PlayerBusy = false;
    }
}
