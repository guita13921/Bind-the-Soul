using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public System.Func<CharacterData, string> name;
    public System.Func<CharacterData, bool> isAvailable;
    public System.Action<CharacterData> applyEffect;

    public Buff(
        System.Func<CharacterData, string> name,
        System.Func<CharacterData, bool> isAvailable,
        System.Action<CharacterData> applyEffect
    )
    {
        this.name = name;
        this.isAvailable = isAvailable;
        this.applyEffect = applyEffect;
    }
}
