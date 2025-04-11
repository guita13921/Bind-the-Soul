using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItem : MonoBehaviour
{
    public GameObject spellWarmUpFX;
    public GameObject spellCastFX;
    public string spellAnimation;

    [Header("Spell Type")]
    public bool isFaithSpell;
    public bool isMagicSpell;
    public bool isPyroSpell;

    [Header("Spell Description")]
    [TextArea]
    public string spellDescription;

    public virtual void AttemptToCashSpell()
    {
        Debug.Log("you attemp to cast a spell");
    }

    public virtual void SucessfullyCastSpell()
    {
        Debug.Log("You Sucessfuly cast a spell");
    }
}
