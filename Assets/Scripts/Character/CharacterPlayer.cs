using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assumes singleplayer for this project.
/// </summary>
public class CharacterPlayer : BaseCharacter
{
    public static CharacterPlayer Instance = null;

    public CharacterPlayerController Controller = null;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void Start()
    {
        base.Start();

        EnablePhysics();
        Controller = gameObject.AddComponent<CharacterPlayerController>();
    }

    public override CharacterDirection GetDirection()
    {
        if (Controller.IsMoving())
            return Controller.GetDirection();
        else
        {
            //Don't update direction if we are not moving
            return Direction;
        }
    }

    public override bool IsPlayer()
    {
        return true;
    }

    public override bool IsMoving()
    {
        return Controller.enabled && Controller.IsMoving();
    }
}
