using System.Linq;
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

    private const float m_interactRange = 2.5f;

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

    protected override void Update()
    {
        base.Update();

        if (RootScript.PlayerBusy)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            Interact();

        if(Input.GetKeyDown(KeyCode.I))
        {
            UIManager.ShowPlayerInventory();
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_interactRange);
    }

    //Interact with the nearest interactable.
    public void Interact()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll((Vector2)transform.position, m_interactRange);
        List<InteractableEntity> interactables = new List<InteractableEntity>();

        for(int i = 0; i < colls.Length; i++)
        {
            //Dont interact with our own children.
            if (colls[i].transform.root == transform)
                continue;

            InteractableEntity interactable = colls[i].GetComponent<InteractableEntity>();

            if (interactable != null)
                interactables.Add(interactable);
        }

        if (interactables.Count <= 0)
            return;

        interactables.OrderBy(x => Vector3.Distance(x.transform.position, transform.position));
        interactables[0].OnPlayerInteract();
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
