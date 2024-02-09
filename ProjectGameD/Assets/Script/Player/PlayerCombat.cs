using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerCombat : MonoBehaviour
{

    public List<AttackSO> combo;
    float lastClickedTime; //time betweeen attack in combo 
    float lastComboEnd; //amount of time before player can do the next combo
    int comboCounter;

    float specialAttackCooldown = 5f;
    float timeSinceLastSpecialAttack = 0f;
    bool isSpecialAttackReady = true;
    Animator animator;
    [SerializeField] Weapon weapon;

    public PlayerCD playerCD;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()

    {

        SpecialAttack();
        if(Input.GetKeyDown(KeyCode.J)){
            Attack();
        }
       
        ExitAttack();
    }

    void Attack(){
        if(Time.time - lastComboEnd >0.2f && comboCounter < combo.Count){
            
            CancelInvoke("EndCombo");

            if(Time.time-lastClickedTime >= 0.5f ){
                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                animator.Play("Attack",0,0);
                weapon.damage = combo[comboCounter].damage;
                comboCounter ++;


                lastClickedTime = Time.time;

                if(comboCounter +1 > combo.Count){
                    comboCounter = 0;
                }

            }
        }
    }

    void SpecialAttack()

{




    if (!isSpecialAttackReady)
    {
        timeSinceLastSpecialAttack += Time.deltaTime;
        playerCD.CooldownText(specialAttackCooldown-timeSinceLastSpecialAttack);
        if (timeSinceLastSpecialAttack >= specialAttackCooldown)
        {
            isSpecialAttackReady = true;
            timeSinceLastSpecialAttack = 0f;
            playerCD.cooldownReady();

        }
    }

    if (isSpecialAttackReady && Input.GetKeyDown(KeyCode.K))
    {
        animator.Play("SPAttack", 0, 0);
        isSpecialAttackReady = false;
    }
}

    void ExitAttack(){
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
            Invoke("EndCombo",0.5f);
        }
    }

    

    void EndCombo(){
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}
