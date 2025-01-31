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
            damage += 500;
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
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
            if (enemy.CompareTag("Enemy"))
            {
                projectileAttack.SpwanBull();

                enemy.health.currentHealth -= damage;
            }
        }
    }
}
