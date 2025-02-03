using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSoulbond : MonoBehaviour
{
    public CharacterData characterData; // Reference to CharacterData

    public Soulbond soulbond;

    public void ApplyBuff()
    {
        characterData.ResetToDefault();

        foreach (var soul in soulbond.buffForActive)
        {
            Debug.Log(soul.name);
            soul.applyEffect(characterData);
        }
    }
}
