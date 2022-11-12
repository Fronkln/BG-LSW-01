using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayerController : MonoBehaviour
{
    public float Speed = 155;

    private BaseCharacter m_Player = null;
    private Vector2 m_moveDir = Vector2.zero;

    private void Awake()
    {
        m_Player = GetComponent<BaseCharacter>();
    }

    private void Update()
    {
        InputUpdate();
    }

    private void FixedUpdate()
    {
        m_Player.Rigidbody.velocity = (m_moveDir * Speed) * Time.deltaTime;
    }

    public void InputUpdate()
    {
        m_moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public CharacterDirection GetDirection()
    {
        if (m_moveDir == Vector2.zero)
            return CharacterDirection.Down;

        //Prioritize left/right over up and down animations.
        //I prefer it like this, the opposite can be done of course.

        if (m_moveDir.x < 0)
            return CharacterDirection.Left;
        else if(m_moveDir.x > 0)
            return CharacterDirection.Right;

        if (m_moveDir.y > 0)
            return CharacterDirection.Up;
        else
            return CharacterDirection.Down;
    }

    public bool IsMoving()
    {
        return m_moveDir != Vector2.zero;
    }
}
