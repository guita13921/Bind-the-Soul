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
    [SerializeField] private float berserkAttackCooldownMultiplier = 0.1f; 

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
        //Debug.Log("CheckHealthForBerserk");
        if (!isBerserk && health.GetCurrentHealth() <= health.GetMaxHealth() * 0.5f)
        {
            EnterBerserkMode();
        }
    }

    private void SetAnimationSpeed(float speedMultiplier)
    {
        if (animator != null)
        {
            animator.speed = speedMultiplier; // Scale the speed of all animations
            //Debug.Log($"Animation speed set to {animator.speed}");
        }
    }

    private void EnterBerserkMode()
    {
        isBerserk = true;
        //Debug.Log("Berserk Mode Activated!");

        //Stat Config
        speed *= berserkSpeedMultiplier; // Increase movement speed
        weapon.damage += berserkDamageBoost; // Boost damage
        CoolDownAttack *= berserkAttackCooldownMultiplier; // Reduce attack cooldown
        animator.SetTrigger("Berserk"); // Play Berserk animation
        animator.SetBool("IsBerserk",true); // Play Berserk animation


        //Speed of Animation
        SetAnimationSpeed(animationSpeedMultiplier * 1.25f); // Make animations faster in Berserk mode
    }

    void OnTriggerEnter(Collider other){
        if(jumphitbox.isTrigger && other.gameObject.CompareTag("Player") && isBerserk){
            agent.transform.LookAt(player.transform);
            //Debug.Log("JumpAttack");
            animator.SetTrigger("JumpAttack");
            
            state = State.Cooldown;
        }else{
            return;
        }
    }
}