using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull_attack : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider;
    Animator animator;

    void Start(){
        animator = GetComponent<Animator>();
    }

    void EnableAttack(){
        boxCollider.enabled = true;
    }

    void DisableAttack(){
        boxCollider.enabled = false;
    }
}
