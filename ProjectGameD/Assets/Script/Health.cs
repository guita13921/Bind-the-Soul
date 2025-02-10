using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public CharacterData characterData;
    void Start()
    {
    currentHealth = maxHealth;
        //currentHealth = characterData.Health;
    }
}
