using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combo;
    public GameObject[] vfxPrefabs; // Array to hold references to VFX prefabs
    public GameObject[] specialVfxPrefabs;

    public GameObject[] sKillVFX;
        public GameObject[] specialSKillVFX;



    float lastClickedTime; //time betweeen attack in combo
    float lastComboEnd; //amount of time before player can do the next combo
    int comboCounter;

    float specialAttackCooldown = 5f;
    float timeSinceLastSpecialAttack = 0f;
    bool isSpecialAttackReady = true;
    Animator animator;

    [SerializeField]
    PlayerWeapon weapon;
    public SFX sfx;
    [SerializeField] BoxCollider boxCollider;
    public Transform parentObject; // The object inside which you want to spawn the new object
 
    bool normalmode =true;
    public PlayerCD playerCD;

    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()


    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if(normalmode){
            normalmode =false;}else{
                normalmode = true;
            }
        }


        SpecialAttack();
        if (Input.GetKeyDown(KeyCode.J) && !animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit"))
        {
            Attack();
        }

        ExitAttack();
    }

    void Attack()
    {
        //if(Time.time - lastComboEnd >0.2f && comboCounter < combo.Count){

        CancelInvoke("EndCombo");

        if (
            Time.time - lastClickedTime >= 0.3f
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("SPAttack")
        )
        {
            animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
            animator.Play("Attack", 0, 0);
            sfx.Slash();
             if (vfxPrefabs != null && vfxPrefabs.Length > 0 )
        {
            if(normalmode){
            GameObject vfxPrefab = vfxPrefabs[comboCounter]; 
            Instantiate(vfxPrefab,parentObject);
            }else{
            GameObject vfxPrefab = specialVfxPrefabs[comboCounter]; 
            Instantiate(vfxPrefab,parentObject);
            }

           }
            weapon.damage = combo[comboCounter].damage;
            comboCounter++;

            lastClickedTime = Time.time;

            if (comboCounter + 1 > combo.Count)
            {
                comboCounter = 0;
            }
            
            // Spawn VFX
           
        }
        //}
    }

    void SpecialAttack()
    {
        if (!isSpecialAttackReady)
        {
            timeSinceLastSpecialAttack += Time.deltaTime;
            playerCD.CooldownText(specialAttackCooldown - timeSinceLastSpecialAttack);
            if (timeSinceLastSpecialAttack >= specialAttackCooldown)
            {
                isSpecialAttackReady = true;
                timeSinceLastSpecialAttack = 0f;
                playerCD.cooldownReady();
            }
        }

        if (isSpecialAttackReady && Input.GetKeyDown(KeyCode.K))
        {
            if(normalmode){
            GameObject vfxPrefab = sKillVFX[0]; 
            Instantiate(vfxPrefab,parentObject);
            }else{
            GameObject vfxPrefab = specialSKillVFX[0]; 
            Instantiate(vfxPrefab,parentObject);
            }
                        sfx.SkillSlash();

            animator.Play("SPAttack", 0, 0);
            isSpecialAttackReady = false;
        }
    }

    void ExitAttack()
    {
        if (
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f
            && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        )
        {
            Invoke("EndCombo", 0.5f);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
    void EnableAttack(){
        boxCollider.enabled = true;
    }

    void DisableAttack(){
        boxCollider.enabled = false;
    }
   


}
