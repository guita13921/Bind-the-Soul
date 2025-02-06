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

    void Start()
    {
        // Find the first active PlayerCombat in the scene
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
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
                    damage - reducedDamage - reducedDamageDependOnHP
                );
            }
        }
    }
}
