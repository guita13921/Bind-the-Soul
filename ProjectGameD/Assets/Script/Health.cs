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
    public CharacterData characterData; // Ensure this is assigned

    void Start()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log("Scene Name: " + sceneName);

        if (sceneName == "Stage01")
        {
            currentHealth = maxHealth;
        }
        else
        {
            if (characterData != null)
            {
                currentHealth = characterData.currentHP;
            }
            else
            {
                Debug.LogError("CharacterData is null!");
            }
        }
    }
}
