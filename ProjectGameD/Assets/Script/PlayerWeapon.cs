using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage;
    float damageR = 0f;
    public CharacterData characterData;

    [SerializeField]
    bool isQK = true;

    [SerializeField]
    ProjectileAttack projectileAttack;

    void Start()
    {
        if (isQK && characterData.Q1_QKDamageUp)
        {
            damage += damage * 0.25f;
            damageR = damage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isQK && characterData.Q2_QKCrit)
        {
            damage = damageR;

            int randomValue = UnityEngine.Random.Range(0, 10);
            if (randomValue < 2) // 20%
            {
                damage *= 3;
            }
        }

        var enemy = other.gameObject.GetComponent<EnemyHealth>();
        var enemyai3 = other.gameObject.GetComponent<EnemyAI3>();
        if (enemy != null)
        {
            if (other.CompareTag("Enemy"))
            {
                if (characterData.Q3_QKSlow && isQK)
                {
                    enemyai3.FixSpeed();
                }
                projectileAttack.SpwanBull();
                enemy.CalculateDamage(damage);
            }
        }
    }
}
