using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI3 : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] protected GameObject player;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] protected EnemyWeapon weapon;
    [SerializeField] protected Animator animator;
    [SerializeField] protected LayerMask groundLayer, playerLayer;
    [SerializeField] protected DissolvingControllerTut dissolvingControllerTut;

    [Header("Movement Control")]
    [SerializeField] Rigidbody rb;
    [SerializeField] protected float speed;
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] private protected float range;
    [SerializeField] private protected float sightRange, attackRange;
    [SerializeField] private protected bool playerInsight, PlayerInAttackrange;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;
    private bool isDashing = false;


    [SerializeField]
    public enum State
    {
        Ready,
        Cooldown,
        KnockBack,
        Dead,
    };

    [Header("StateManagement")]
    [SerializeField] public State state;
    [SerializeField] protected float timerCoolDownAttack = 0;
    bool timerReachedCoolDownAttack = false;
    [SerializeField] protected float timerCoolKnockBack = 0;
    bool timerReachedCoolKnockBack = false;
    private protected float KnockBackTime;
    private protected float CoolDownAttack;
    [SerializeField] private int numberOfRandomVariations;
    private int currentBehaviorType = -1;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 2f; // Freeze duration
    [SerializeField] private GameObject spawnEffect; // VFX Graph effect
    [SerializeField] protected bool isSpawning = true;
    [SerializeField] private float effectSizeMultiplier = 2f; // Multiplier for size

    [Header("VFX")]
    [SerializeField] private GameObject hitEffectPrefab; // Prefab for the hit effect
    [SerializeField] private Transform hitEffectSpawnPoint; // Where the effect spawns (optional)
    [SerializeField] private float hitEffectDuration = 1f; // Time before destroying the prefab

    [Header("Heath/Canvas")]
    [SerializeField] GameObject bar;
    [SerializeField] protected EnemyHealth health;
    [SerializeField] PlayerWeapon playerWeapon;
    [SerializeField] Canvas attackIndicatorCanvas;
    [SerializeField] AttackIndicatorController attackIndicatorController;

    //Temp
    [SerializeField] protected float originalSpeed = 0f;
    private bool isSlowingDown = false;
    private float slowTimer = 0f;

    public void FixSpeed()
    {
        if (!isSlowingDown)
            speed = speed * 0.7f;
        slowTimer = 2f;
        isSlowingDown = true;
    }

    public void setoriginalspeed()
    {
        originalSpeed = speed;
    }

    protected virtual void Start()
    {
        InitializeComponents();
        StartCoroutine(HandleSpawn());
    }

    private void InitializeComponents()
    {
        dissolvingControllerTut = GetComponent<DissolvingControllerTut>();
        playerWeapon = GameObject.Find("PlayerSwordHitbox").GetComponent<PlayerWeapon>();
        state = State.Ready;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        weapon = GetComponentInChildren<EnemyWeapon>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "HitVFXSpawnplaceReal")
            {
                boxCollider = child.GetComponent<BoxCollider>();

                if (boxCollider != null)
                {
                    break;
                }
            }
        }
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator HandleSpawn()
    {
        isSpawning = true;
        agent.enabled = false;
        dissolvingControllerTut.EndDissolve("1");

        if (spawnEffect != null)
        {
            spawnEffect.SetActive(true);
        }

        yield return new WaitForSeconds(spawnDelay);

        agent.enabled = true;
        isSpawning = false;
        bar.SetActive(true);
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
        range = IN_range;
        sightRange = IN_sightRange;
        attackRange = IN_attackRange;
        dashDistance = IN_dashDistance;
        dashSpeed = IN_dashSpeed;
        IN_agent.stoppingDistance = IN_stoprange;
    }

    protected virtual void Update()
    {
        // //agent.speed = speed;
        slowTimer -= Time.deltaTime;
        if (slowTimer < 0)
        {
            speed = originalSpeed;
            isSlowingDown = false;
        }

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
        //rb.AddForce(hitDirection.normalized * knockBackForce, ForceMode.Impulse);
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
            //Debug.Log("Selected Behavior Type: " + currentBehaviorType);
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

    public virtual void Dead()
    {
        playerWeapon.killEnemyTimerAdder();
        if (state == State.Dead)
        {
            return;
        }

        state = State.Dead;
        gameObject.tag = "DEAD";

        foreach (Transform child in transform)
        {
            child.gameObject.tag = "DEAD";
        }

        if (bar != null)
        {
            Destroy(bar.gameObject);
        }

        agent.enabled = false;
        animator.SetBool("Death", true);
        attackIndicatorCanvas.gameObject.SetActive(false);
        StartCoroutine(DestroyAfterDelay(5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword") && state != State.Dead && !isSpawning)
        {
            PlayerWeapon playerWeapon = other.gameObject.GetComponent<PlayerWeapon>();
            agent.transform.LookAt(player.transform);
            Vector3 knockBackDirection = transform.position - player.transform.position;
            KnockBack(knockBackDirection, 10f);
        }
    }

    void ResetAttackBehavior()
    {
        currentBehaviorType = -1; // Reset to allow for a new random behavior in the next attack
    }

    public bool GetIsSpawning()
    {
        return isSpawning;
    }
}
