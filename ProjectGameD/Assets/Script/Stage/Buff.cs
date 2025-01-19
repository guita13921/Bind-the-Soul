using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public System.Func<CharacterData, string> name;
    public System.Func<CharacterData, bool> isAvailable;
    public System.Action<CharacterData> applyEffect;
    public System.Func<CharacterData, string> description;
    public System.Func<CharacterData, string> type;

    public Buff(
        System.Func<CharacterData, string> name,
        System.Func<CharacterData, bool> isAvailable,
        System.Action<CharacterData> applyEffect,
        System.Func<CharacterData, string> description,
        System.Func<CharacterData, string> type
    )
    {
        this.name = name;
        this.isAvailable = isAvailable;
        this.applyEffect = applyEffect;
        this.description = description;
        this.type = type;
    }
}
