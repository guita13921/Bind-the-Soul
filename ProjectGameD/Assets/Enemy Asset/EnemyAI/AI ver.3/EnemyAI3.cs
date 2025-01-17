using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyAI3 : MonoBehaviour
{
    [SerializeField]
    protected GameObject player;

    [SerializeField]
    protected NavMeshAgent agent;

    [SerializeField]
    protected LayerMask groundLayer,
        playerLayer;

    [SerializeField]
    Rigidbody rb;

    //Walk Var
    Vector3 destPoint;
    bool walkpointSet;

    [SerializeField]
    private protected float range;

    [SerializeField]
    private protected float sightRange,
        attackRange;

    [SerializeField]
    private protected bool playerInsight,
        PlayerInAttackrange;

    //Animatotion var
    protected Animator animator;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    protected Weapon_Enemy weapon;

    //Attack Forward
    [SerializeField]
    private float dashDistance;

    [SerializeField]
    private float dashSpeed;
    private bool isDashing = false;

    //State Var
    public enum State
    {
        Ready,
        Cooldown,
        KnockBack,
        Dead,
    };

    [SerializeField]
    public State state;

    [SerializeField]
    protected float speed;

    //CoolDown var
    [SerializeField]
    protected float timerCoolDownAttack = 0;
    bool timerReachedCoolDownAttack = false;

    [SerializeField]
    protected float timerCoolKnockBack = 0;
    bool timerReachedCoolKnockBack = false;
    private protected float KnockBackTime;
    private protected float CoolDownAttack;

    //Attack Aniamtion
    [SerializeField]
    private int numberOfRandomVariations;
    private int currentBehaviorType = -1; // Default to -1 indicating no behavior set yet

    // Spawn Effect Variables
    [Header("Spawn Settings")]
    [SerializeField]
    private float spawnDelay = 2f; // Freeze duration

    [SerializeField]
    private VisualEffect spawnEffect; // VFX Graph effect

    [SerializeField]
    protected bool isSpawning = true;

    [SerializeField]
    private float effectSizeMultiplier = 2f; // Multiplier for size

    // Hit Effect
    [SerializeField]
    private GameObject hitEffectPrefab; // Prefab for the hit effect

    [SerializeField]
    private Transform hitEffectSpawnPoint; // Where the effect spawns (optional)

    [SerializeField]
    private float hitEffectDuration = 1f; // Time before destroying the prefab

    //Heath and Canvas
    [SerializeField]
    Canvas bar;

    [SerializeField]
    protected EnemyHealth health;

    [SerializeField]
    PlayerWeapon playerWeapon;

    [SerializeField]
    Canvas attackIndicatorCanvas;

    [SerializeField]
    AttackIndicatorController attackIndicatorController;

    protected virtual void Start()
    {
        InitializeComponents();
        StartCoroutine(HandleSpawn());
    }

    private void InitializeComponents()
    {
        playerWeapon = GameObject.Find("PlayerSwordHitbox").GetComponent<PlayerWeapon>();
        state = State.Ready;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        weapon = GetComponentInChildren<Weapon_Enemy>();
        boxCollider = GetComponentInChildren<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator HandleSpawn()
    {
        isSpawning = true;
        agent.enabled = false;

        // Set the size of the VFX effect
        if (spawnEffect != null)
        {
            spawnEffect.Play();
        }

        yield return new WaitForSeconds(spawnDelay);

        if (spawnEffect != null)
        {
            spawnEffect.Stop();
        }

        agent.enabled = true;
        isSpawning = false;
    }

    public virtual void SetStat(
        float IN_knockBackTime,
        float IN_coolDownAttack,
        int IN_randomVariations,
        int IN_Speed,
        int IN_Damage,
        int IN_range,
        int IN_sightRange,
        int IN_attackRange,
        float IN_dashDistance,
        float IN_dashSpeed,
        int IN_stoprange,
        NavMeshAgent IN_agent
    )
    {
        KnockBackTime = IN_knockBackTime;
        CoolDownAttack = IN_coolDownAttack;
        numberOfRandomVariations = IN_randomVariations;
        speed = IN_Speed;
        IN_agent.speed = speed;
        //weapon.damage = IN_Damage;
        range = IN_range;
        sightRange = IN_sightRange;
        attackRange = IN_attackRange;
        dashDistance = IN_dashDistance;
        dashSpeed = IN_dashSpeed;
        IN_agent.stoppingDistance = IN_stoprange;
    }

    protected virtual void Update()
    {
        if (isSpawning || state == State.Dead)
            return;

        CheckHealth();
        AnimationCheckState();
        CooldownKnockBackTime();
        CoolDownAttaickTime();

        // Check player visibility and distance
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        // Only proceed with movement logic if the NavMeshAgent is active and placed on a NavMesh
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (
                !playerInsight
                && !PlayerInAttackrange
                && state != State.KnockBack
                && state != State.Cooldown
            )
                Patrol();
            if (
                playerInsight
                && !PlayerInAttackrange
                && state != State.KnockBack
                && state != State.Cooldown
            )
                Chase();
            if (playerInsight && PlayerInAttackrange && state == State.Ready)
                Attack();
        }
        else
        {
            Debug.LogWarning(
                "NavMeshAgent is not active or is not on a NavMesh. Movement disabled."
            );
        }
    }

    protected virtual void AnimationCheckState()
    {
        if (state == State.Cooldown)
        {
            animator.SetBool("IsCooldown", true);
            DisableAttack(0);
        }
        else
        {
            animator.SetBool("IsCooldown", false);
        }

        if (state == State.KnockBack)
        {
            DisableAttack(0);
            animator.SetBool("isKnockBack", true);
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);
        }
        else
        {
            animator.SetBool("isKnockBack", false);
        }
    }

    protected virtual void CheckHealth()
    {
        if (health.GetCurrentHealth() <= 0 && state != State.Dead)
        {
            Dead();
        }
    }

    protected virtual void CoolDownAttaickTime()
    {
        if (!timerReachedCoolDownAttack && state == State.Cooldown)
            timerCoolDownAttack += Time.deltaTime;
        if (
            !timerReachedCoolDownAttack
            && timerCoolDownAttack > CoolDownAttack
            && state == State.Cooldown
        )
        { //#############
            agent.speed = speed;
            state = State.Ready;
            timerCoolDownAttack = 0;
        }
    }

    private protected virtual void CooldownKnockBackTime()
    {
        if (!timerReachedCoolKnockBack && state == State.KnockBack)
            timerCoolKnockBack += Time.deltaTime;
        if (
            !timerReachedCoolKnockBack
            && timerCoolKnockBack > KnockBackTime
            && state == State.KnockBack
        )
        { //#############
            agent.speed = speed;
            state = State.Ready;
            timerCoolKnockBack = 0;
        }
    }

    private protected virtual void KnockBack(Vector3 hitDirection, float knockBackForce)
    {
        state = State.KnockBack;
        animator.SetTrigger("Knockback");
        rb.AddForce(hitDirection.normalized * knockBackForce, ForceMode.Impulse);
    }

    public void Chase()
    {
        DisableAttack(0);
        animator.SetBool("Chase", true);
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        if (currentBehaviorType == -1)
        {
            currentBehaviorType = UnityEngine.Random.Range(0, numberOfRandomVariations);
            animator.SetInteger("AttackType", currentBehaviorType);
            Debug.Log("Selected Behavior Type: " + currentBehaviorType);
        }

        animator.SetBool("Chase", false);
        //agent.transform.LookAt(player.transform);
        animator.SetBool("Attack", true);
    }

    private IEnumerator DashForward()
    {
        isDashing = true;

        Vector3 dashDirection = transform.forward;
        float reducedDashDistance = dashDistance;
        Vector3 potentialTargetPosition = transform.position + dashDirection * reducedDashDistance;

        // Project the target position onto the NavMesh
        if (
            NavMesh.SamplePosition(
                potentialTargetPosition,
                out NavMeshHit hit,
                reducedDashDistance,
                NavMesh.AllAreas
            )
        )
        {
            Vector3 targetPosition = hit.position; // Use the position on the NavMesh
            float dashTime = Vector3.Distance(transform.position, targetPosition) / dashSpeed; // Adjust dash time
            float startTime = Time.time;

            // While the enemy has not reached the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                float journeyProgress = (Time.time - startTime) / dashTime;
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    journeyProgress
                );
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Dash target position is not on the NavMesh. Cancelling dash.");
        }

        isDashing = false;
        //StopAttack(); // Optional
    }

    void Dead()
    {
        if (state == State.Dead)
        {
            return;
        }
        else
        {
            state = State.Dead;

            // Change the tag of the enemy and all its children to "DEAD"
            gameObject.tag = "DEAD"; // Change the tag of the parent (enemy)

            // Iterate over all children and set their tag to "DEAD"
            foreach (Transform child in transform)
            {
                child.gameObject.tag = "DEAD";
            }

            // Destroy the health bar
            Destroy(bar.gameObject);

            // Disable NavMeshAgent
            agent.enabled = false;

            // Play death animation
            animator.SetBool("Death", true);

            // Deactivate the attack indicator canvas
            attackIndicatorCanvas.gameObject.SetActive(false);
        }
    }

    protected virtual void Patrol()
    {
        if (!walkpointSet)
            SearchForDest();
        if (walkpointSet)
            agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10)
            walkpointSet = false;
    }

    void SearchForDest()
    {
        float z = UnityEngine.Random.Range(-range, range);
        float x = UnityEngine.Random.Range(-range, range);

        destPoint = new Vector3(
            transform.position.x + x,
            transform.position.y,
            transform.position.z + z
        );

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void EnableAttack()
    {
        if (!isDashing)
        {
            StartCoroutine(DashForward());
        }
        HideIndicator();
        boxCollider.enabled = true;
    }

    void DisableAttack(int AttackTimeFrame)
    {
        HideIndicator();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
            if (AttackTimeFrame != 0)
            {
                ShowIndicator(AttackTimeFrame);
            }
        }
    }

    void ShowIndicator(int AttackTimeFrame)
    {
        if (attackIndicatorController != null)
        {
            //attackIndicatorCanvas.enabled = true;
            attackIndicatorController.ShowIndicator(AttackTimeFrame);
        }
    }

    void HideIndicator()
    {
        if (attackIndicatorController != null)
        {
            attackIndicatorController.HideIndicator();
        }
    }

    void StartAttack(int AttackTimeFrame)
    {
        this.ShowIndicator(AttackTimeFrame);
        agent.transform.LookAt(player.transform);
        agent.speed = 0;
    }

    void StopAnimation()
    {
        agent.speed = speed;
    }

    void StartAnimation()
    {
        agent.speed = 0;
    }

    void StopAttack()
    {
        animator.SetBool("Attack", false);
        state = State.Cooldown;
        ResetAttackBehavior();
    }

    void StartKnockBack()
    {
        agent.speed = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword") && state != State.Dead)
        {
            //Debug.Log("Damage");
            health.CalculateDamage(playerWeapon.damage);
            agent.transform.LookAt(player.transform);
            Vector3 knockBackDirection = transform.position - player.transform.position;
            KnockBack(knockBackDirection, 10f);
            PlayHitEffect();
        }
    }

    public GameObject[] sound;
    public GameObject dmgtext;

    private void PlayHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            // Determine the spawn position (use spawn point or fallback to the enemy's position)
            Vector3 spawnPosition =
                (hitEffectSpawnPoint != null) ? hitEffectSpawnPoint.position : transform.position;

            // Instantiate the hit effect prefab
            GameObject hitEffectInstance = Instantiate(
                hitEffectPrefab,
                spawnPosition,
                Quaternion.identity
            );
            if (sound != null && sound.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, sound.Length);
                GameObject soundObject = sound[randomIndex];
                GameObject swordhit = Instantiate(soundObject);
                AudioSource audioSource = soundObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
            if (playerWeapon != null)
            {
                float damage = playerWeapon.damage;
                Debug.Log(damage);
                Vector3 newPosition = this.transform.position;
                newPosition.y += 1;
                GameObject dmg = Instantiate(dmgtext, newPosition, Quaternion.Euler(0, 60, 0));

                damageShow damageTextComponent = dmg.GetComponent<damageShow>();
                if (damageTextComponent != null)
                {
                    damageTextComponent.SetDamage(damage);
                }
            }
            // Optionally, parent it to the enemy (useful for effects like blood dripping)
            hitEffectInstance.transform.SetParent(transform);

            // Destroy the effect after a set duration
            Destroy(hitEffectInstance, hitEffectDuration);
        }
    }

    void ResetAttackBehavior()
    {
        currentBehaviorType = -1; // Reset to allow for a new random behavior in the next attack
    }
}
