using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage;
    public CharacterData characterData;
    private float reducedDamageSecond = 0; // if HP < 25% of maxHP
    public PlayerCombat playerCombat;
    public float reduceDamageTimer = 0f;
    public bool reducedDamage = false;
    float damageR = 0f;
    [SerializeField]bool isCurseAttack = false;
    void Start()
    {
        // Find the first active PlayerCombat in the scene
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    void Update()
    {
        if (reducedDamage)
        {
            reduceDamageTimer -= Time.deltaTime;
            if (reduceDamageTimer < 0)
            {
                reducedDamage = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
   
        damageR = damage;
        if (playerCombat != null){
            if(playerCombat.gotCurse) damage *= 2;}
     if(isCurseAttack){
            playerCombat.GotCurse();
        }

        if (reducedDamage)
        {
            damage -= damage * 0.3f;
        }

        Health player = other.gameObject.GetComponent<Health>();
        if (player != null && other.CompareTag("Player"))
        {
            if (!playerCombat.isShield1 && !playerCombat.isShield2)
            {
                if (player.currentHealth < (player.maxHealth * 0.25f))
                {
                    reducedDamageSecond = characterData.reduceIncomeDamageDependOnHP * 0.15f; // 0.15f per level (15%, 30%, 45%)
                }
                float damageReductionPercentage = characterData.reduceIncomeDamage * 0.05f; // 0.05f per level (5%, 10%, 15%)
                float reducedDamage = damage * damageReductionPercentage;
                float reducedDamageDependOnHP = damage * reducedDamageSecond;
                player.currentHealth -= Mathf.Max(
                    0,
    Mathf.RoundToInt(damage - reducedDamage - reducedDamageDependOnHP)
                    
                );
            }
        }
        damage = damageR;
    }
/*
    private void OnTriggerStay (Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            var HP = other.gameObject.GetComponent<Health>();
            //Debug.Log("OnTriggerStay");
            HP.currentHealth -= damage;
        }
    }
*/
}
