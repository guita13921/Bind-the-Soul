using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Chaos_warriors : EnemyAI3
{
    private bool isBerserk = false;

    //Stat Config
    [SerializeField] private float berserkSpeedMultiplier = 1f;
    [SerializeField] private int berserkDamageBoost = 10;
    [SerializeField] private float berserkAttackCooldownMultiplier = 3f; 

    [SerializeField] BoxCollider jumphitbox;

    //Aniamtion Config
    [SerializeField] private float animationSpeedMultiplier = 1; // Multiplier for animation speed

    protected override void Start()
    {
        base.Start(); // Call the base Start() method
        SetAnimationSpeed(1); // Add custom logic
        agent.transform.LookAt(player.transform);
    }

    protected override void Update()
    {
        //Debug.Log("Update");
        CheckHealthForBerserk(); // Custom behavior for Chaos_Warriro
        base.Update(); // Retain the base functionality
    }
    

    private void CheckHealthForBerserk()
    {
        if (!isBerserk && health.GetCurrentHealth() <= health.GetMaxHealth() * 0.5f){
            EnterBerserkMode();
        }
    }

    private void SetAnimationSpeed(float speedMultiplier)
    {
        if (animator != null)
        {
            animator.speed = speedMultiplier;
        }
    }

    private void EnterBerserkMode()
    {
        isBerserk = true;
        //Debug.Log("Berserk Mode Activated!");

        //Stat Config
        agent.speed = 3; // Increase movement speed
        Debug.Log("speed : " + CoolDownAttack);
        weapon.damage += berserkDamageBoost; // Boost damage
        CoolDownAttack = 3f; // Reduce attack cooldown
        Debug.Log("CoolDownAttack : " + CoolDownAttack);
        animator.SetTrigger("Berserk"); // Play Berserk animation
        animator.SetBool("IsBerserk",true); // Play Berserk animation
        SetAnimationSpeed(animationSpeedMultiplier * 1.25f); // Make animations faster in Berserk mode
    }

    void OnTriggerEnter(Collider other){
        if(jumphitbox.isTrigger && other.gameObject.CompareTag("Player") && isBerserk){
            agent.transform.LookAt(player.transform);
            animator.SetTrigger("JumpAttack");
            state = State.Cooldown;
        }else{
            return;
        }
    }

    void StartJumpAttack(){

    }

    void StopJumpAttack(){

    }
}