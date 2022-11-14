using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    IEnumerator DoSpeechRoutine(int index, float speed)
    {
        int count = m_speech[index].Length;
        float interval = speed / count;

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(interval);

            m_speechText.text += m_speech[index][i];
            m_soundEmitter.clip = m_typeSound;

            if (!m_soundEmitter.isPlaying)
            {
                m_soundEmitter.pitch = UnityEngine.Random.Range(0.85f, 1);
                m_soundEmitter.Play();
            }
        }
    }

    public void Update()
    {
        if (!m_active)
            return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
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
        m_speechText.text = "";
        m_speakerText.text = m_speaker[index];

        if (m_typeSpeed[index] <= 0)
            m_speechText.text = m_speech[index];
        else
            StartCoroutine(DoSpeechRoutine(index, m_typeSpeed[index]));
    }
}
