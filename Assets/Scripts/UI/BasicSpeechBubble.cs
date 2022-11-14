using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Speech bubble UI that toggles its visibility based on whether a speech is currently happening right now.
/// Pretty hacky solution. But can't make a elegant solution for everything when i have 72 hours to go.
/// </summary>
public class BasicSpeechBubble : MonoBehaviour
{
    private SpriteRenderer m_Sprite;

    void Start()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        m_Sprite.enabled = !UIManager.IsSpeechActive();
    }
}
