using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechTrigger : InteractableEntity
{
    public SpeechData Data = new SpeechData();

   [SerializeField] private bool m_onlyOnce = false;

    public override void OnPlayerInteract()
    {
        base.OnPlayerInteract();

        UIManager.DoSpeech(Data.Speech, Data.Speaker, Data.Speed);

        if (m_onlyOnce)
            Destroy(gameObject);
    }
}
