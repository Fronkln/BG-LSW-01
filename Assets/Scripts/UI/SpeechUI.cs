using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class SpeechUI : MonoBehaviour
{
    //UI Objects
    private TextMeshProUGUI m_speakerText;
    private TextMeshProUGUI m_speechText;
    private AudioSource m_soundEmitter;

    //Sounds
    [SerializeField] private AudioClip m_typeSound;

    //Variables
    private bool m_active = false;

    private int m_curSpeechIndex = 0;
    private float[] m_typeSpeed;

    private string[] m_speech = null;
    private string[] m_speaker = null;

    private Action m_speechFinishedCallback = null;

    private Coroutine m_speechCoroutine;

    //The speech function used to be a coroutine, but it proved to be difficult to manipulate
    //So i moved it to Update instead
    private int m_targetCount;
    private int m_curLetter;
    private float m_interval;
    private float m_curInterval;

    public void Awake()
    {
        m_speakerText = transform.Find("SpeechInside/Speaker").GetComponent<TextMeshProUGUI>();
        m_speechText = transform.Find("SpeechInside/Speech").GetComponent<TextMeshProUGUI>();
        m_soundEmitter = transform.GetComponent<AudioSource>();
    }

    public void Initialize(string[] speech, string[] speaker, float[] typeDuration = null, Action finishCallback = null)
    {
        if (typeDuration == null || typeDuration.Length == 0)
        {
            typeDuration = new float[speech.Length];
            Array.Fill(typeDuration, 0);
        }

        RootScript.PlayerBusy = true;

        m_speech = speech;
        m_speaker = speaker;
        m_curSpeechIndex = 0;
        m_typeSpeed = typeDuration;
        m_active = true;

        m_speechFinishedCallback = finishCallback;

        gameObject.SetActive(true);
        ShowDialogue(0);

    }

    private void Update()
    {
        if (!m_active)
            return;

        if (m_typeSpeed[m_curSpeechIndex] > 0)
            m_curInterval += Time.deltaTime;

        if (RootScript.ConfirmKeyIsPressed())
        {
            if (m_curLetter != m_targetCount)
            {
                m_curLetter = m_targetCount;
                m_speechText.text = m_speech[m_curSpeechIndex];
            }
            else
            {
                m_curSpeechIndex++;

                if (m_curSpeechIndex >= m_speech.Length)
                    OnDialogueFinished();
                else
                    ShowDialogue(m_curSpeechIndex);
            }
        }
    }

    public void FixedUpdate()
    {
        if (!m_active)
            return;

        if (m_typeSpeed[m_curSpeechIndex] > 0)
            UpdateTyping();
    }

    private void UpdateTyping()
    {
        if (m_curLetter < m_targetCount)
        {
            if (m_curInterval >= m_interval)
            {
                m_speechText.text += m_speech[m_curSpeechIndex][m_curLetter];
                m_soundEmitter.clip = m_typeSound;

                if (!m_soundEmitter.isPlaying)
                {
                    m_soundEmitter.pitch = UnityEngine.Random.Range(0.85f, 1);
                    m_soundEmitter.Play();
                }

                m_curInterval = 0;
                m_curLetter++;
            }
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
        m_speechText.text = "";
        m_speakerText.text = m_speaker[index];

        if (m_typeSpeed[index] <= 0)
            m_speechText.text = m_speech[index];
        else
        {
            m_curInterval = 0;
            m_curLetter = 0;
            m_targetCount = m_speech[index].Length;
            m_interval = m_typeSpeed[index] / m_targetCount;
        }
    }
}
