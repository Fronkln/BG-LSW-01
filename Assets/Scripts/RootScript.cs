using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script responsible for controlling important variables
/// </summary>
public static class RootScript
{
    /// <summary>
    /// Player is busy doing something, block interactions like opening menus
    /// <br>or moving</br>
    /// </summary>
    public static bool PlayerBusy = false;
}
