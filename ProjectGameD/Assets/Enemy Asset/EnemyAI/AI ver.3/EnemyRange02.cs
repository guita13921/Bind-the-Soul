using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRange02 : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] public EnemyRange02_Animation enemyAnimation;
    [SerializeField] public EnemyHealth health;
    [SerializeField] Rigidbody rb;

    [Header("Movement")]
    public Transform player;
    public float patrolSpeed = 3f;
    [SerializeField] public float detectionRange;
    [SerializeField] public float attackRange;
    public LayerMask obstacleMask;

    [Header("Fireball Attack")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float attackCooldown;
    public Vector2 uiOffset;

    [Header("Hiding")]
    public float hideDistance = 10f;
    public float hideSearchRadius = 15f;

    [Header("Stun")]
    [SerializeField] public float stunDuration = 3f; 

    private NavMeshAgent agent;
    [SerializeField] private float attackCooldownTimer = 0f;
    private bool isStunned = false;
    [SerializeField] private bool isOnAttackCooldown = false;
    private bool isShooting = false;
    private bool isHiding = false;

    [SerializeField] private int numberOfBullets = 3;
    [SerializeField] private float bulletDelay = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isStunned) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && !isOnAttackCooldown && !isShooting)
        {
            PatrolToPlayer();

            if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0)
            {
                enemyAnimation.PlayAttackAnimation();
                StartShoot();
            }
        }
        else if (isOnAttackCooldown)
        {
            FindHidingSpot();
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        CheckNearbyObjects();
    }

    void PatrolToPlayer()
    {
        if (!Physics.Linecast(transform.position, player.position, obstacleMask))
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(player.position);
        }
    }

    IEnumerator ShootWithDelay(int numberOfBullets, float bulletDelay)
    {
        isShooting = true;
        for (int i = 0; i < numberOfBullets; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(bulletDelay);
        }

        isShooting = false;
        attackCooldownTimer = attackCooldown;
        StartCoroutine(AttackCooldown());
    }

    void StartShoot()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
        }
    }

    private void ShootBullet()
    {
        GameObject projectile = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player.transform, (Vector3)uiOffset);
    }

    void FindHidingSpot()
    {
        Debug.Log("FindHidingSpot");

        Collider[] nearbySpots = Physics.OverlapSphere(transform.position, hideSearchRadius, obstacleMask);

        Transform bestSpot = null;
        float bestScore = float.MinValue;

        foreach (var spot in nearbySpots)
        {
            float distanceToPlayer = Vector3.Distance(spot.transform.position, player.position);
            float distanceToEnemy = Vector3.Distance(spot.transform.position, transform.position);

            if (distanceToPlayer >= hideDistance && distanceToEnemy <= hideSearchRadius)
            {
                float score = distanceToPlayer - distanceToEnemy;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestSpot = spot.transform;
                }
            }
        }

        if (bestSpot != null)
        {
            Debug.Log($"Best hiding spot found: {bestSpot.name}");
            agent.SetDestination(bestSpot.position);
            isHiding = true;
        }
        else
        {
            Debug.Log("No valid hiding spot found");
            isHiding = false;
        }
    }

    void CheckNearbyObjects()
    {
        if (isHiding)
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (var obj in nearbyObjects)
            {
                if (obj.CompareTag("DD") && obj.gameObject.layer == LayerMask.NameToLayer("Can't pass"))
                {
                    enemyAnimation.PlayIdleAnimation();
                    break;
                }
            }
        }
    }

    public void GetHit()
    {
        if (isStunned) return;

        StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        isStunned = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        agent.isStopped = false;

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        isOnAttackCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        isOnAttackCooldown = false;
    }

    public bool GetIsShooting()
    {
        return isShooting;
    }

    public bool GetIsOnAttackCooldown()
    {
        return isOnAttackCooldown;
    }

    public bool GetIsStunned()
    {
        return isStunned;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword") && health.GetCurrentHealth() != 0)
        {
            PlayerWeapon playerWeapon = other.gameObject.GetComponent<PlayerWeapon>();
            if (playerWeapon != null)
                health.CalculateDamage(playerWeapon.damage);

            agent.transform.LookAt(player.transform);
            Vector3 knockBackDirection = transform.position - player.transform.position;
            KnockBack(knockBackDirection, 10f);
        }
    }

    
    private protected virtual void KnockBack(Vector3 hitDirection, float knockBackForce)
    {
        rb.AddForce(hitDirection.normalized * knockBackForce, ForceMode.Impulse);
    }
}
