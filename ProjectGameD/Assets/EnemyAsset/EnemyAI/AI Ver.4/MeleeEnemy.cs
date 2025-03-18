using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            enemyAnimation.PlayAttackAnimation();
            enemyVFX.PlayAttackEffect();
        }
    }

    public override void Chase()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, 2f * Time.deltaTime);
        }
    }

    public override void Defend()
    {
        if (hasShield && shieldHealth > 0)
        {
            enemyAnimation.PlayShieldAnimation();
            enemyVFX.PlayShieldEffect();
        }
    }

    public override void TakeDamage(float amount)
    {
        if (hasShield && shieldHealth > 0)
        {
            shieldHealth -= amount;
            enemyVFX.PlayShieldHitEffect();
        }
        else
        {
            health -= amount;
            enemyVFX.PlayDamageEffect();
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
