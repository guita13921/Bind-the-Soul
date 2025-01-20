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

    [SerializeField]
    CharacterData characterData;

    void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (health.currentHealth <= 0 && !hasDied)
        {
            characterData.deathCount += 1;
            animator.Play("die", 0, 0);
            gameObject.tag = "Untagged";
            gameObject.layer = default;
            hasDied = true;
        }
        else if (hasDied && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            // Lock the animation at the end
            animator.enabled = false;
        }
    }
}
