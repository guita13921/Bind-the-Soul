using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class playerdie : MonoBehaviour
{
    private Health health;
    private Animator animator;
    private bool hasDied = false;
    public GameObject currentSoundtrack;

    [SerializeField]
    CharacterData characterData;

    [SerializeField]
    GameObject deathsound;

    [SerializeField]
    GameObject deathsound_BG;

    void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.X))
        // {
        //     health.currentHealth = 0;
        // }
        if (health.currentHealth <= 0 && !hasDied)
        {
            if (currentSoundtrack != null)
            {
                AudioSource audioSource = currentSoundtrack.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.volume = 0f; // Set volume to zero
                    // audioSource.Stop(); // Uncomment if you want to stop the sound instead
                }
            }
            Instantiate(deathsound);
            Instantiate(deathsound_BG);

            characterData.deathCount += 1;

    animator.SetBool("IsDying", true);
            

            gameObject.tag = "Untagged";
            gameObject.layer = default;
            hasDied = true;

            DisableAllScripts(); // Disable all scripts

        }
        else if (hasDied && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            // Lock the animation at the end
            animator.enabled = false;
            DisableAllScripts();
        }
    }

        void DisableAllScripts()
    {
        // Loop through all MonoBehaviour components and disable them
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
    }
}
