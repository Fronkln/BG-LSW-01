using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LSW 01/Item/Outfit Item")]
public class ItemDefinitionOutfit : ItemDefinition
{
    public override ItemType Type => ItemType.Outfit;

    public BodypartID Bodypart;

    public uint Set = 0;
    public uint Variant = 0;

    public override bool OnUseItem(BaseCharacter usingCharacter)
    {
        usingCharacter.Appearance.SetBodyPart(Bodypart, Set, Variant);
        usingCharacter.Appearance.UpdateAppearance();
        return true;
    }
}
