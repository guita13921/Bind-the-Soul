using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage;
    public Health health;
    public CharacterData characterData;
    private float reducedDamageSecond = 0; // if HP < 25% of maxHP

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<Health>();
        if (player != null)
        {
            if (player.CompareTag("Player"))
            {
                if (health.currentHealth < (health.maxHealth * 0.25))
                {
                    reducedDamageSecond = characterData.reduceIncomeDamageDependOnHP * 0.15f; // 0.15f per level (15%, 30%, 45%)
                }
                float damageReductionPercentage = characterData.reduceIncomeDamage * 0.05f; // 0.05f per level (5%, 10%, 15%)
                float reducedDamage = damage * damageReductionPercentage;
                float reducedDamageDependOnHP = damage * reducedDamageSecond;
                player.currentHealth -= damage - reducedDamage - reducedDamageDependOnHP;
            }
        }
    }
}
