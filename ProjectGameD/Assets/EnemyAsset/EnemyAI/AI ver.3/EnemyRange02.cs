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
    [SerializeField] public bool isDead = false;
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
    [SerializeField] public float spawnDelay = 2f; // Freeze duration
    [SerializeField] public GameObject spawnEffect; // VFX Graph effect
    public bool isSpawning = true;

    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public float attackCooldownTimer = 0f;
    [SerializeField] public bool isStunned = false;
    [SerializeField] public bool isOnAttackCooldown = false;
    [SerializeField] public bool isShooting = false;
    [SerializeField] public bool isHiding = false;

    [SerializeField] public int numberOfBullets = 3;
    [SerializeField] public float bulletDelay = 0.5f;



    // Idle tracking
    public Vector3 previousPosition;
    public float idleTimer = 0f;
    public const float idleThreshold = 0.001f; // Movement threshold

    public virtual void Start()
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

    public virtual void Update()
    {
        if(player != null){
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
        }

        //HandleIdleAnimation();
    }

    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    /*
    public virtual void HandleIdleAnimation()
    {
        if (Vector3.Distance(transform.position, previousPosition) < idleThreshold && !isSpawning)
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
    */

    public virtual void PatrolToPlayer()
    {
        if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask) && isSpawning == false && isDead == false)
        {
            if(agent != null){
                agent.speed = patrolSpeed;
                agent.SetDestination(player.transform.position);
            }
        }
    }

    public virtual IEnumerator ShootWithDelay(int numberOfBullets, float bulletDelay)
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

    public void StartShoot()
    {
        if (!isShooting)
        {
            RequestInsideLookAtPlayer();
            StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
        }
    }

    public virtual void ShootBullet()
    {
        GameObject projectile = Instantiate(BulletPrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player, (Vector3)uiOffset);
    }

    public virtual void FindHidingSpot()
    {
        Debug.Log("FindHidingSpot");
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

    public virtual void GetHit()
    {
        if (isStunned) return;
        StartCoroutine(Stun());
    }

    public virtual IEnumerator Stun()
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

    public virtual IEnumerator AttackCooldown()
    {
        isOnAttackCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        isOnAttackCooldown = false;
    }

    public bool GetIsShooting() => isShooting;
    public virtual bool GetIsSpawning() => isSpawning;
    public virtual bool GetIsOnAttackCooldown() => isOnAttackCooldown;
    public virtual bool GetIsStunned() => isStunned;

    public virtual void OnTriggerEnter(Collider other)
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
        StartCoroutine(DestroyAfterDelay(5f));
    }

    
    public virtual void RequestInsideLookAtPlayer()
    {
        if (player == null) return; // Ensure the player reference exists

        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0; // Keep rotation only on the Y-axis

        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer); // Instantly face the player
        }
    }


}