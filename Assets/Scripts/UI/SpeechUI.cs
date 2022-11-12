using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechUI : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI Speaker;
    [HideInInspector] public TextMeshProUGUI Speech;

    public void Awake()
    {
        Speaker = transform.Find("SpeechInside/Speaker").GetComponent<TextMeshProUGUI>();
        Speech = transform.Find("SpeechInside/Speech").GetComponent<TextMeshProUGUI>();
    }
}
