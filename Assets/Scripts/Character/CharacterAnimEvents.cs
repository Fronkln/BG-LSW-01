using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterAnimEvents: Used in animation clips to manage the character.
/// </summary>
public class CharacterAnimEvents : MonoBehaviour
{
    private BaseCharacter m_Character;

    private void Awake()
    {
        m_Character = GetComponent<BaseCharacter>();
    }

    public void SetFrame(int frame)
    {
        m_Character.Appearance.SetAnimationFrame(frame);
    }
}
