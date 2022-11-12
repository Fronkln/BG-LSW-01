using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechUI : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI Speaker;
    [HideInInspector] public TextMeshProUGUI Speech;

    private bool m_active = false;

    private int m_curSpeechIndex = 0;

    private string[] m_speech = null;
    private string[] m_speaker = null;

    private Action m_speechFinishedCallback = null;

    public void Awake()
    {
        Speaker = transform.Find("SpeechInside/Speaker").GetComponent<TextMeshProUGUI>();
        Speech = transform.Find("SpeechInside/Speech").GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(string[] speech, string[] speaker, Action finishCallback = null)
    {
        RootScript.PlayerBusy = true;

        m_speech = speech;
        m_speaker = speaker;
        m_curSpeechIndex = 0;
        m_active = true;

        m_speechFinishedCallback = finishCallback;

        ShowDialogue(0);

        gameObject.SetActive(true);
    }

    public void Update()
    {
        if (!m_active)
            return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            m_curSpeechIndex++;

            if (m_curSpeechIndex >= m_speech.Length)
                OnDialogueFinished();
            else
                ShowDialogue(m_curSpeechIndex);
        }
    }

    private void OnDialogueFinished()
    {
        m_active = false;
        m_speechFinishedCallback?.Invoke();
        RootScript.PlayerBusy = false;

        gameObject.SetActive(false);
    }

    private void ShowDialogue(int index)
    {
        Speech.text = m_speech[index];
        Speaker.text = m_speaker[index];
    }
}
