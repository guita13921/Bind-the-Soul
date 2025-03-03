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
    [SerializeField] bool isDead = false;
    [SerializeField] Canvas bar;

    [Header("Movement")]
    public GameObject player;
    public float patrolSpeed;
    [SerializeField] public float detectionRange;
    [SerializeField] public float attackRange;
    public LayerMask obstacleMask;

    [Header("Fireball Attack")]
    public GameObject BulletPrefab;
    public Transform firePoint;
    public float attackCooldown;
    public Vector2 uiOffset;

    [Header("Hiding")]
    public float hideDistance;
    public float hideSearchRadius;

    [Header("Stun")]
    [SerializeField] public float stunDuration; 

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 2f; // Freeze duration
    protected bool isSpawning = true;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackCooldownTimer = 0f;
    private bool isStunned = false;
    [SerializeField] private bool isOnAttackCooldown = false;
    protected bool isShooting = false;
    protected bool isHiding = false;

    [SerializeField] private int numberOfBullets = 3;
    [SerializeField] private float bulletDelay = 0.5f;

    // Idle tracking
    private Vector3 previousPosition;
    private float idleTimer = 0f;
    private const float idleThreshold = 0.01f; // Movement threshold

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAnimation = GetComponent<EnemyRange02_Animation>();
        agent = GetComponent<NavMeshAgent>();  // Assign first
        health = GetComponent<EnemyHealth>();
        StartCoroutine(HandleSpawn());         // Start coroutine after assignment
        previousPosition = transform.position; // Initialize position tracker
    }

    public virtual void SetStat(
        float IN_knockBackTime,
        float IN_attackCooldown,
        int IN_Speed,
        int IN_Damage,
        int IN_range,
        int IN_sightRange,
        int IN_attackRange,
        float IN_dashDistance,
        float IN_dashSpeed,
        int IN_stoprange,
        NavMeshAgent IN_agent,
        int IN_hideDistance,
        int IN_hideSearchRadius
    ){
        stunDuration = IN_knockBackTime;
        attackCooldown = IN_attackCooldown;
        patrolSpeed = IN_Speed;
        agent.speed = IN_Speed;
        detectionRange = IN_sightRange;
        attackRange = IN_attackRange;
        IN_agent.stoppingDistance = IN_stoprange;
        hideDistance = IN_hideDistance;
        hideSearchRadius = IN_hideSearchRadius;
    }

    private IEnumerator HandleSpawn()
    {
        isSpawning = true;
        agent.enabled = false;
        yield return new WaitForSeconds(spawnDelay);
        agent.enabled = true;
        isSpawning = false;
    }

    protected virtual void Update()
    {
        if(isDead || isSpawning) return;

        if (health.GetCurrentHealth() == 0)
        {   
            Dead();
        }

        if (isStunned) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRange && !isOnAttackCooldown && !isShooting)
        {
            PatrolToPlayer();

            if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0)
            {
                enemyAnimation?.PlayAttackAnimation();
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

        HandleIdleAnimation();
    }

    void HandleIdleAnimation()
    {
        if (Vector3.Distance(transform.position, previousPosition) < idleThreshold)
        {
            idleTimer += Time.deltaTime;
            enemyAnimation?.PlayIdleAnimation();
        }
        else
        {
            enemyAnimation?.PlayEndIdleAnimation();
            idleTimer = 0f; // Reset timer if movement detected
        }

        previousPosition = transform.position; // Update position for next frame
    }

    void PatrolToPlayer()
    {
        if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask) && isSpawning == false)
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(player.transform.position);
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

    private void StartShoot()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
        }
    }

    private void ShootBullet()
    {
        GameObject projectile = Instantiate(BulletPrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player, (Vector3)uiOffset);
    }

    void FindHidingSpot()
    {
        Collider[] nearbySpots = Physics.OverlapSphere(transform.position, hideSearchRadius, obstacleMask);

        Transform bestSpot = null;
        float bestScore = float.MinValue;

        foreach (var spot in nearbySpots)
        {
            float distanceToPlayer = Vector3.Distance(spot.transform.position, player.transform.position);
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
            agent.SetDestination(bestSpot.position);
            isHiding = true;
        }
        else
        {
            isHiding = false;
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
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        isOnAttackCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        isOnAttackCooldown = false;
    }

    public bool GetIsShooting() => isShooting;
    public bool GetIsSpawning() => isSpawning;
    public bool GetIsOnAttackCooldown() => isOnAttackCooldown;
    public bool GetIsStunned() => isStunned;

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword") && health != null && health.GetCurrentHealth() != 0)
        {
            PlayerWeapon playerWeapon = other.gameObject.GetComponent<PlayerWeapon>();
            if (playerWeapon != null)
                GetHit();
                //health.CalculateDamage(playerWeapon.damage);

            agent.transform.LookAt(player.transform);
            enemyAnimation?.PlayStunAnimation();
        }
    }

    public virtual void Dead()
    {
        gameObject.tag = "DEAD"; 
        isDead = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true; // Stop agent before disabling
        }

        foreach (Transform child in transform)
        {
            child.gameObject.tag = "DEAD";
        }

        Destroy(bar.gameObject);

        if (agent != null)
        {
            agent.enabled = false; // Disable after stopping
        }

        enemyAnimation?.PlayDeadAniamtion();
    }
}