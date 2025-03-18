using UnityEngine;
using UnityMovementAI;
using System.Collections.Generic;


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
            Vector3 accel = collisionAvoidance.GetSteering(nearbyObstacles);
            accel += wallAvoidance.GetSteering();

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
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

    private void OnTriggerEnter(Collider other)
    {
        MovementAIRigidbody obstacle = other.GetComponent<MovementAIRigidbody>();
        if (obstacle != null && !nearbyObstacles.Contains(obstacle))
        {
            nearbyObstacles.Add(obstacle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MovementAIRigidbody obstacle = other.GetComponent<MovementAIRigidbody>();
        if (obstacle != null && nearbyObstacles.Contains(obstacle))
        {
            nearbyObstacles.Remove(obstacle);
        }
    }
}
