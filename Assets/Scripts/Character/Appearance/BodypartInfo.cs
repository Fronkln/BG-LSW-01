using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Information regarding the bodypart belonging to a character.
/// </summary>
//Not a struct so we can modify this as we need (with struct we will get a compiler error!)
public class BodypartInfo
{
    public uint Set;
    public uint Variation;
}
