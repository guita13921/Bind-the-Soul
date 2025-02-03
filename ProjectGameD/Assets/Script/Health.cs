using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public CharacterData characterData;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Stage01")
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = characterData.currentHP;
        }
    }
}
