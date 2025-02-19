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

    public static bool killEnemy;
    public float killEnemyTimer = 0f;

    void Start()
    {
        if (isQK && characterData.Q1_QKDamageUp)
        {
            damage += damage * 0.25f;
        }

        if (killEnemy)
        {
            damage = Mathf.Round(damage + damage * 0.3f);
        }
    }

    void Update()
    {
        if (killEnemy)
        {
            killEnemyTimer -= Time.deltaTime;
            if (killEnemyTimer < 0)
            {
                killEnemy = false;
            }
        }
    }

    public void killEnemyTimerAdder()
    {
        if (characterData.Q1_QKKillEnemyDamageUp)
        {
            killEnemyTimer = 3f;
            killEnemy = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        damageR = damage;

        if (killEnemy && !isQK)
            damage = Mathf.Round(damage + damage * 0.3f);

        if (isQK && characterData.Q2_QKCrit)
        {
            int randomValue = UnityEngine.Random.Range(0, 10);
            if (randomValue < 2) // 20%
            {
                damage *= 3;
            }
        }

        var enemy = other.gameObject.GetComponent<EnemyHealth>();
        var enemyai3 = other.gameObject.GetComponent<EnemyAI3>();
        var hitDetection = other.gameObject.GetComponentInChildren<HitDetection>();
        if (enemy != null)
        {
            if (other.CompareTag("Enemy"))
            {
                if (characterData.Q3_QKSlow && isQK)
                {
                    enemyai3.FixSpeed();
                }
                if (projectileAttack)
                    projectileAttack.SpwanBull();

                enemy.CalculateDamage(damage, isQK, characterData.Q3_QKWeak);
                if (hitDetection)
                    hitDetection.SpanwDamageText(damage);
            }
        }
        damage = damageR;
    }
}
