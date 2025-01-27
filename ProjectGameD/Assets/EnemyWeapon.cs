using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage;
    public CharacterData characterData;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<Health>();
        if (player != null)
        {
            if (player.CompareTag("Player"))
            {
                float damageReductionPercentage = characterData.reduceIncomeDamage * 0.05f; // 0.05f per level (5%, 10%, 15%)
                float reducedDamage = damage * damageReductionPercentage;

                player.currentHealth -= damage - reducedDamage;
            }
        }
    }
}
