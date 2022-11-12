using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple follow camera, good enough.
/// </summary>
public class SimpleFollowCamera : MonoBehaviour
{
    public Transform Target;

    private void Start()
    {
        //Target will be spawned by then. Let's track em!
        Target = CharacterPlayer.Instance.transform;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, transform.position.z);
    }
}
