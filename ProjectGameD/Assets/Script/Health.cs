using UnityEngine;
using UnityEngine.SceneManagement; // Import the SceneManager namespace

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public CharacterData characterData;
    public NextStage nextStage;

    void Start()
    {
        maxHealth = characterData.maxHealth;
        currentHealth = characterData.maxHealth * characterData.healthRatio;
    }
}
