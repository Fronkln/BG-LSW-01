using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// BaseCharacter: The class all characters inherit from
/// </summary>
public class BaseCharacter : MonoBehaviour
{
    /// <summary>
    /// Module responsible for controlling character appearance.
    /// </summary>
    [HideInInspector] public CharacterAppearance Appearance = null;
    [HideInInspector] public Rigidbody2D Rigidbody = null;
    [HideInInspector] public Animator Animator = null;
    [HideInInspector] public BoxCollider2D Collider = null;

    public Inventory Inventory = new Inventory();

    public CharacterDirection Direction = CharacterDirection.Down;

    public virtual void Awake()
    {
        Appearance = gameObject.AddComponent<CharacterAppearance>();

        //Physics component of the character.
        Rigidbody = gameObject.AddComponent<Rigidbody2D>();
        Rigidbody.gravityScale = 0; //In a top down game not doing this means falling down
        Rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous; //Top notch collisions for the player
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation; //Rotating us is not rigidbody's business this time!

        //Animator component of the character.
        Animator = gameObject.AddComponent<Animator>();
        Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("HumanGeneric");

        //Collider to not go through things.
        Collider = gameObject.AddComponent<BoxCollider2D>();

        //Since all characters are one and the same, this hardcoded value will be just fine.
        Collider.offset = new Vector2(-0.007904053f, -0.03000623f);
        Collider.size = new Vector2(1.125187f, 1.626777f);

        //Enables animations to more easily interact with the character.
        gameObject.AddComponent<CharacterAnimEvents>();

        //We are not physically simulated by default (all constraints enabled), the player will change that.
        DisablePhysics();
    }

    public virtual void Start()
    {

    }

    protected virtual void Update()
    {
        Direction = GetDirection();
        AnimationUpdate();
    }

    public virtual void FixedUpdate()
    {
        //Prevents characters from sliding eachother to the oblivion
        if (Rigidbody.simulated)
            if (!IsMoving())
                Rigidbody.velocity = Vector2.zero;
    }

    public virtual void AnimationUpdate()
    {
        Animator.SetBool("moving", IsMoving());
        Appearance.SetFlipped(Direction == CharacterDirection.Left); //flip anim if we are left
    }

    //The direction the character is facing.
    public virtual CharacterDirection GetDirection()
    {
        return Direction;
    }

    public virtual bool IsPlayer()
    {
        return false;
    }

    public virtual bool IsMoving()
    {
        return false;
    }

    public void EnablePhysics()
    {
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void DisablePhysics()
    {
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
