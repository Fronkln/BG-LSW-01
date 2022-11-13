using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class CharacterAppearance : MonoBehaviour
{
    /// <summary>
    /// The character this component belongs to.
    /// </summary>
    public BaseCharacter Character;

    /// <summary>
    /// A clean, fast way to access the bodyparts of a character.
    /// The value on the dictionary refers to the variation ID
    /// The way it will access sprites from the sprite atlas is: variation_(id)_(direction)_(frame)
    /// </summary>
    private Dictionary<BodypartID, BodypartInfo> m_bodypartInfo = new Dictionary<BodypartID, BodypartInfo>();

    //The bodypart sprite objects found within the character.
    private Dictionary<BodypartID, SpriteRenderer> m_bodypartSprites = new Dictionary<BodypartID, SpriteRenderer>();

    //The bodypart sprite atlas used for easy access to character textures/animation.
    private Dictionary<BodypartID, SpriteAtlas> m_bodypartAtlas = new Dictionary<BodypartID, SpriteAtlas>();

    /// <summary>
    /// Maximum amount of bodyparts a character can consist of.
    /// </summary>
    public static readonly int BodypartCount = Enum.GetNames(typeof(BodypartID)).Length;
    private const float BODYPART_SIZE = 1;

    private void Awake()
    {
        Character = GetComponent<BaseCharacter>();

        InitializeDefaultAppearance();
        GenerateSprites();

        UpdateAppearance();
    }

    private void InitializeDefaultAppearance()
    {
        //Add the default values for each body part
        for (int i = 0; i < BodypartCount; i++)
        {
            BodypartID part = (BodypartID)i;

            m_bodypartInfo[part] = new BodypartInfo();
            m_bodypartSprites[part] = null;
            m_bodypartAtlas[part] = null;
        }
    }

    private void ClearSprites()
    {
        //Clear previous sprite objects if we had any
        foreach (var kv in m_bodypartSprites)
            if (kv.Value != null)
                Destroy(kv.Value);

        m_bodypartSprites.Clear();
    }

    //Create the sprite objects that will be used for character's appearance.
    //We store these sprites as a children of the character object.
    private void GenerateSprites()
    {
        ClearSprites();

        for(int i = 0; i < BodypartCount; i++)
        {
            BodypartID part = (BodypartID)i;

            //Create the object
            GameObject spriteObject = new GameObject(part.ToString());
            spriteObject.transform.parent = transform;
            spriteObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            spriteObject.transform.localScale = new Vector3(BODYPART_SIZE, BODYPART_SIZE, BODYPART_SIZE);

            //Create the sprite component
            SpriteRenderer bodypartRenderer = spriteObject.AddComponent<SpriteRenderer>();

            m_bodypartSprites[part] = bodypartRenderer;
        }
    }

    private void UpdateAppearance()
    {
        //Iterate through every body part and update the characters current appearance
        foreach(var kv in m_bodypartInfo)
        {
            BodypartID part = kv.Key;
            BodypartInfo info = kv.Value;

            SpriteRenderer bodypartRenderer = m_bodypartSprites[part];
            bodypartRenderer.sortingLayerName = "Player";

            string atlasName = $"{part.ToString().ToLowerInvariant()}_set{info.Set}";
            SpriteAtlas partAtlas = Resources.Load<SpriteAtlas>(atlasName);

            if (partAtlas != null)
                m_bodypartAtlas[part] = partAtlas;
            else
                Debug.LogError("Missing atlas in resources: " + atlasName);
        }

        //Set the sprites accordingly.
        SetTexture(CharacterDirection.Down, 0);
    }


    /// <summary>
    /// Get information about the bodypart (set, variation)
    /// </summary>
    public BodypartInfo GetBodypartInformation(BodypartID bodyPart)
    {
        return m_bodypartInfo[bodyPart];
    }

    /// <summary>
    /// Change the appearance of a bodypart.
    /// </summary>
    public void SetBodyPart(BodypartID body, uint set, uint variation)
    {
        m_bodypartInfo[body].Set = set;
        m_bodypartInfo[body].Variation = variation;
    }

    public void SetBodyPart(BodypartID body, uint set, int variation) => SetBodyPart(body, set, (uint)variation);

    private string GetStringFromDirection(CharacterDirection direction)
    {
        switch(direction)
        {
            default:
                throw new Exception("Invalid direction");
            case CharacterDirection.Left:
                return "lft";
            case CharacterDirection.Right:
                goto case CharacterDirection.Left;
            case CharacterDirection.Up:
                return "up";
            case CharacterDirection.Down:
                return "dwn";
        }
    }

    private Sprite GetTextureForDirection(BodypartID part, CharacterDirection direction, int frame)
    {
        if (m_bodypartAtlas[part] == null)
            return null;

        BodypartInfo bodypartInfo = m_bodypartInfo[part];

        //Convention: variation(id)_(direction)_(frame)
        string spriteName = $"variant{bodypartInfo.Variation}_{GetStringFromDirection(direction)}_{frame}";
        return m_bodypartAtlas[part].GetSprite(spriteName);
    }

    /// <summary>
    /// Gets all sprites used by this character, one use case: Inventory player visualization
    /// </summary>
    public Sprite[] GetTextures(CharacterDirection direction, int frame = 0)
    {
        List<Sprite> sprites = new List<Sprite>();

        for(int i = 0; i < BodypartCount; i++)
            sprites.Add(GetTextureForDirection((BodypartID)i, direction, frame));

        return sprites.ToArray();
    }

     /// <summary>
     /// Set the direction and the frame of the character.
     /// </summary>
     /// <param name="direction"></param>
    public void SetTexture(CharacterDirection direction, int frame)
    {
        foreach(var kv in m_bodypartSprites)
        {
            BodypartID part = kv.Key;
            m_bodypartSprites[part].sprite = GetTextureForDirection(part, direction, frame);
        }
    }

    /// <summary>
    /// Set the animation frame of the character. Primarily intended for animation event use
    /// </summary>
    public void SetAnimationFrame(int frame)
    {
        SetTexture(Character.Direction, frame);
    }

    public void SetFlipped(bool flipped)
    {
        foreach (var kv in m_bodypartSprites)
            kv.Value.flipX = flipped;
    }
}
