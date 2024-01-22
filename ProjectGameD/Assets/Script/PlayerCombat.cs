using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerCombat : MonoBehaviour
{

    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    Animator animator;
    [SerializeField] Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.J)){
            Attack();
        }
        ExitAttack();
    }

    void Attack(){
        if(Time.time - lastComboEnd >0.2f && comboCounter <= combo.Count){
            
            CancelInvoke("EndCombo");

            if(Time.time-lastClickedTime >= 0.8f ){

                comboCounter ++;
                if(comboCounter > combo.Count){
                    comboCounter = 0;
                }
                
                animator.Play("Attack",0,0);

                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;


                //weapon.damage = combo[comboCounter].damage;

                lastClickedTime = Time.time;

                if(comboCounter > combo.Count){
                    comboCounter = 0;
                }

            }
        }
    }

    void ExitAttack(){
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
            Invoke("EndCombo",1);
        }
    }

    

    void EndCombo(){
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}
