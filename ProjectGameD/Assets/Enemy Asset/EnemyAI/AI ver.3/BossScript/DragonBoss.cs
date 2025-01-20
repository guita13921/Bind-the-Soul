using UnityEngine;
using UnityEngine.AI;


public class DragonBoss : MonoBehaviour
{
    // Enum for boss phases
    public enum BossPhase
    {
        Phase1,  // Grounded phase
        Phase2,  // Aerial phase
        Enraged, // Optional enraged state
        Dead
    }

    // Current phase of the boss
    [SerializeField] private BossPhase currentPhase = BossPhase.Phase1;

    [Header("General Settings")]
    [SerializeField] private float maxHealth = 1000f;
    private float currentHealth;

    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private NavMeshAgent agent;

    [Header("Phase 1 Settings")]
    [SerializeField] private float phase1AttackRange = 10f;
    [SerializeField] private float phase1FireBreathCooldown = 5f;

    [Header("Phase 2 Settings")]
    [SerializeField] private float flightHeight = 20f;
    [SerializeField] private float phase2FireRainCooldown = 7f;

    [Header("Enrage Settings")]
    [SerializeField] private bool enableEnrage = true;
    [SerializeField] private float enrageThreshold = 0.25f; // Health percentage to trigger enraged state

    private bool isEnraged = false;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem fireBreathEffect;

    private float attackTimer = 0f;

    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Ensure references
        if (!player) player = GameObject.FindWithTag("Player").transform;
        if (!agent) agent = GetComponent<NavMeshAgent>();

        // Transition to Phase 1
        TransitionToPhase(BossPhase.Phase1);
    }

    private void Update()
    {
        // Handle states
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                HandlePhase1();
                break;

            case BossPhase.Phase2:
                HandlePhase2();
                break;

            case BossPhase.Enraged:
                HandleEnraged();
                break;

            case BossPhase.Dead:
                // Do nothing if the boss is dead
                break;
        }

        // Check for phase transitions
        if (currentHealth <= 0 && currentPhase != BossPhase.Dead)
        {
            TransitionToPhase(BossPhase.Dead);
        }
        else if (enableEnrage && !isEnraged && currentHealth <= maxHealth * enrageThreshold)
        {
            TransitionToPhase(BossPhase.Enraged);
        }
    }

    /// <summary>
    /// Handles the behavior for Phase 1.
    /// </summary>
    private void HandlePhase1()
    {
        if (player == null) return;

        // Get direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Get the angle between the dragon's forward direction and the player
        float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

        // Determine if the dragon should move forward, turn left, or turn right
        if (Mathf.Abs(angleToPlayer) < 10f) // Small angle, move forward
        {
            MoveForward();
        }
        else if (angleToPlayer > 10f) // Positive angle, turn right
        {
            TurnRight();
        }
        else if (angleToPlayer < -10f) // Negative angle, turn left
        {
            TurnLeft();
        }

        // Smoothly rotate toward the player
        RotateTowardsPlayer(directionToPlayer);
    }

    private void MoveForward()
    {
        // Play the move animation
        animator.SetBool("IsMoving", true);
        animator.SetBool("IsTurningLeft", false);
        animator.SetBool("IsTurningRight", false);

        // Move the dragon forward
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.position);
        }
    }

    private void TurnLeft()
    {
        // Play the turn left animation
        animator.SetBool("IsTurningLeft", true);
        animator.SetBool("IsTurningRight", false);
        animator.SetBool("IsMoving", false);
    }

    private void TurnRight()
    {
        // Play the turn right animation
        animator.SetBool("IsTurningRight", true);
        animator.SetBool("IsTurningLeft", false);
        animator.SetBool("IsMoving", false);
    }

    private void RotateTowardsPlayer(Vector3 directionToPlayer)
    {
        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate toward the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * agent.angularSpeed);
    }

    /// <summary>
    /// Handles the behavior for Phase 2.
    /// </summary>
    private void HandlePhase2()
    {
        if (player == null) return;

        // Hover in the air
        Vector3 hoverPosition = new Vector3(player.position.x, flightHeight, player.position.z);
        transform.position = Vector3.Lerp(transform.position, hoverPosition, Time.deltaTime);

        // Attack the player
        AttackPhase2();
    }

    /// <summary>
    /// Handles the behavior for the enraged phase.
    /// </summary>
    private void HandleEnraged()
    {
        Debug.Log("Boss is enraged! Increased aggression!");

        // Increase attack speed or frequency
        phase1FireBreathCooldown *= 0.75f;
        phase2FireRainCooldown *= 0.75f;

        // Behave based on current phase
        if (currentPhase == BossPhase.Phase1) HandlePhase1();
        if (currentPhase == BossPhase.Phase2) HandlePhase2();
    }

    /// <summary>
    /// Triggers an attack in Phase 1.
    /// </summary>
    private void AttackPhase1()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= phase1FireBreathCooldown)
        {
            attackTimer = 0f;

            // Fire breath attack
            animator.SetTrigger("FireBreath");
            if (fireBreathEffect != null) fireBreathEffect.Play();

            Debug.Log("Dragon uses Fire Breath in Phase 1!");
        }
    }

    /// <summary>
    /// Triggers an attack in Phase 2.
    /// </summary>
    private void AttackPhase2()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= phase2FireRainCooldown)
        {
            attackTimer = 0f;

            // Fire rain attack
            animator.SetTrigger("FireRain");
            Debug.Log("Dragon uses Fire Rain in Phase 2!");
        }
    }

    /// <summary>
    /// Transitions the boss to a new phase.
    /// </summary>
    /// <param name="newPhase">The phase to transition to.</param>
    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;

        switch (newPhase)
        {
            case BossPhase.Phase1:
                Debug.Log("Boss transitioned to Phase 1: Grounded.");
                agent.enabled = true;
                break;

            case BossPhase.Phase2:
                Debug.Log("Boss transitioned to Phase 2: Aerial.");
                agent.enabled = false;
                break;

            case BossPhase.Enraged:
                Debug.Log("Boss is now Enraged!");
                isEnraged = true;
                break;

            case BossPhase.Dead:
                HandleDeath();
                break;
        }
    }

    /// <summary>
    /// Handles the boss's death.
    /// </summary>
    private void HandleDeath()
    {
        Debug.Log("Boss has been defeated!");
        animator.SetTrigger("Death");

        // Disable all behaviors
        agent.enabled = false;
        enabled = false;

        // Optionally add death effects or loot drops
        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// Reduces the boss's health.
    /// </summary>
    /// <param name="damage">Amount of damage to deal.</param>
    public void TakeDamage(float damage)
    {
        if (currentPhase == BossPhase.Dead) return;

        currentHealth -= damage;
        Debug.Log($"Boss Health: {currentHealth}/{maxHealth}");
    }
}
