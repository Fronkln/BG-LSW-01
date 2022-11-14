using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script responsible for controlling important variables
/// </summary>
public class RootScript : MonoBehaviour
{
    /// <summary>
    /// Player is busy doing something, block interactions like opening menus
    /// <br>or moving</br>
    /// </summary>
    public static bool PlayerBusy = false;

    public void Awake()
    {
        Screen.SetResolution(800, 600, false);
    }

    public static bool ConfirmKeyIsPressed()
    {
        return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return);
    }
}
