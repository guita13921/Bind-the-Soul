using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public float attackRange;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    private EnemyMovement enemyMovement;
    [SerializeField] float distanceToTarget;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float ShieldDistance;

    protected override void Start()
    {
        base.Start();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        if (target != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            enemyMovement.CheckDistance(stoppingDistance, distanceToTarget);
            if (distanceToTarget <= ShieldDistance)
            {
                Defend();
            }
            else if (distanceToTarget <= attackRange)
            {
                Attack(); // Attack if within range
            }
            else if (distanceToTarget > stoppingDistance)
            {
                Chase(); // Otherwise, chase the target
            }
        }
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
        enemyMovement.Chase(); // Delegate movement to EnemyMovement script
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
