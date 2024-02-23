using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerdie : MonoBehaviour
{
    private Health health;
    private Animator animator;
    private bool hasDied = false;

    void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

    }
    void Update(){

        if (health.currentHealth <=0 && !hasDied)
        {
            animator.Play("die", 0, 0);
            hasDied =true;
        }
    }

    
}
