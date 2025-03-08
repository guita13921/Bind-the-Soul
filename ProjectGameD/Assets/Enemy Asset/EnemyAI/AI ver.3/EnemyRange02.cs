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
    [SerializeField] protected bool isDead = false;
    [SerializeField] GameObject bar;

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
    [SerializeField] private GameObject spawnEffect; // VFX Graph effect
    protected bool isSpawning = true;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected float attackCooldownTimer = 0f;
    [SerializeField] protected bool isStunned = false;
    [SerializeField] protected bool isOnAttackCooldown = false;
    [SerializeField] protected bool isShooting = false;
    [SerializeField] protected bool isHiding = false;

    [SerializeField] private int numberOfBullets = 3;
    [SerializeField] private float bulletDelay = 0.5f;



    // Idle tracking
    private Vector3 previousPosition;
    private float idleTimer = 0f;
    private const float idleThreshold = 0.01f; // Movement threshold

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if(player == null){
            player = GameObject.FindGameObjectWithTag("Dummy");
        }

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
        spawnEffect.SetActive(true);

        yield return new WaitForSeconds(spawnDelay);

        agent.enabled = true;
        isSpawning = false;
        bar.SetActive(true);
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
        else if (isOnAttackCooldown || isDead == false)
        {
            FindHidingSpot();
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        HandleIdleAnimation();
    }

    protected virtual void HandleIdleAnimation()
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

    protected virtual void PatrolToPlayer()
    {
        if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask) && isSpawning == false && isDead == false)
        {
            if(agent != null){
                agent.speed = patrolSpeed;
                agent.SetDestination(player.transform.position);
            }
        }
    }

    protected virtual IEnumerator ShootWithDelay(int numberOfBullets, float bulletDelay)
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

    protected void StartShoot()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
        }
    }

    protected virtual void ShootBullet()
    {
        GameObject projectile = Instantiate(BulletPrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player, (Vector3)uiOffset);
    }

    protected virtual void FindHidingSpot()
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

        if (bestSpot != null && agent.enabled)
        {
            agent.SetDestination(bestSpot.position);
            isHiding = true;
        }
        else
        {
            isHiding = false;
        }
    }

    protected virtual void GetHit()
    {
        if (isStunned) return;
        StartCoroutine(Stun());
    }

    protected virtual IEnumerator Stun()
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

    protected virtual IEnumerator AttackCooldown()
    {
        isOnAttackCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        isOnAttackCooldown = false;
    }

    public bool GetIsShooting() => isShooting;
    public virtual bool GetIsSpawning() => isSpawning;
    public virtual bool GetIsOnAttackCooldown() => isOnAttackCooldown;
    public virtual bool GetIsStunned() => isStunned;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword") && health != null && health.GetCurrentHealth() != 0 && !isSpawning)
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
        enemyAnimation?.PlayDeadAniamtion();
        Destroy(this.gameObject);
        DisableAllScripts();
    }

    protected virtual void DisableAllScripts()
    {
        // Loop through all MonoBehaviour components and disable them
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
    }

}