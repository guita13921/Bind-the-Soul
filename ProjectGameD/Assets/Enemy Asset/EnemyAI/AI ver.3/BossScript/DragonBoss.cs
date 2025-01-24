using System.Collections;
using System.Collections.Generic; // This is required for List<>
using UnityEngine;

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

    [Header("Phase 1 Settings")]
    [SerializeField] private float phase1AttackCooldown = 10f;

    [Header("Phase 2 Settings")]
    [SerializeField] private float flightHeight = 20f;
    [SerializeField] private float phase2AttackCooldown = 10f;

    [Header("Enrage Settings")]
    [SerializeField] private bool enableEnrage = true;
    [SerializeField] private float enrageThreshold = 0.25f; // Health percentage to trigger enraged state

    private bool isEnraged = false;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem fireBreathEffect;
    private BossRotationWithAnimation movementController;

    private float attackTimer = 0f;

    [Header("Sensor")]
    [SerializeField] private RangeSensor RangeSensor;
    [SerializeField] private MeleeSensor MeleeSensor;

    private enum BossAction
    {
        Backward,
        FireBreath,
        Laser,
        ClawSwipe,
        TailSweep,
        SummonMinions,
        KnockBack
    }

    private List<BossAction> currentCombo = new List<BossAction>(); // Stores the current combo
    private bool isExecutingCombo = false; // Tracks if a combo is currently being executed

    [Header("Agent")]
    [SerializeField] private List<BossAction> combo1 = new List<BossAction> { BossAction.Backward, BossAction.FireBreath };
    [SerializeField] private List<BossAction> combo2 = new List<BossAction> { BossAction.ClawSwipe, BossAction.TailSweep };
    [SerializeField] private List<BossAction> combo3 = new List<BossAction> { BossAction.SummonMinions, BossAction.KnockBack };


    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Ensure references
        if (!player) player = GameObject.FindWithTag("Player").transform;

        movementController = GetComponent<BossRotationWithAnimation>();
        if (!movementController) Debug.LogError("BossRotationWithAnimation script is missing!");

        //agent = GetComponent<NavMeshAgent>();
        //if (!agent) Debug.LogError("NavMeshAgent is missing!");

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

        // Attack logic
        AttackPhase1();
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
        phase1AttackCooldown *= 0.75f;
        phase2AttackCooldown *= 0.75f;

        // Behave based on current phase
        if (currentPhase == BossPhase.Phase1) HandlePhase1();
        if (currentPhase == BossPhase.Phase2) HandlePhase2();
    }

    private int lastMeleeAttackIndex = -1; // Tracks the last melee animation played

    /// <summary>
    /// Triggers an attack in Phase 1.
    /// </summary>
    private void AttackPhase1()
    {
        attackTimer += Time.deltaTime;

        // Perform an attack only after the cooldown
        if (attackTimer >= phase1AttackCooldown && !isExecutingCombo)
        {
            attackTimer = 0f;

            // Decide which combo to execute based on player position
            if (RangeSensor.IsPlayerInRange() && !MeleeSensor.IsPlayerInRange())
            {
                // Player is in RangeSensor but not MeleeSensor
                StartCombo(combo1); // Example: Fire Breath Combo
            }
            else if (MeleeSensor.IsPlayerInRange())
            {
                // Player is in MeleeSensor
                StartCombo(combo2); // Example: Claw Swipe & Tail Sweep Combo
            }
        }
    }

    /// <summary>
    /// Starts executing the given combo.
    /// </summary>
    /// <param name="combo">List of actions to perform.</param>
    private void StartCombo(List<BossAction> combo)
    {
        if (isExecutingCombo) return;

        currentCombo = combo;
        StartCoroutine(ExecuteCombo());
    }

    /// <summary>
    /// Executes the current combo step by step.
    /// </summary>
    private IEnumerator ExecuteCombo()
    {
        isExecutingCombo = true;

        foreach (BossAction action in currentCombo)
        {
            switch (action)
            {
                case BossAction.Backward:
                    Debug.Log("Boss moves backward.");
                    animator.SetTrigger("DashBackward");
                    yield return new WaitForSeconds(1f); // Adjust timing as needed
                    break;

                case BossAction.FireBreath:
                    Debug.Log("Boss performs Fire Breath.");
                    PerformFireBreath(); // Custom fire breath logic
                    yield return new WaitForSeconds(2f); // Adjust timing as needed
                    break;

                case BossAction.Laser:
                    Debug.Log("Boss fires a laser.");
                    animator.SetTrigger("LaserAttack"); // Use appropriate trigger
                    yield return new WaitForSeconds(2f); // Adjust timing as needed
                    break;

                case BossAction.ClawSwipe:
                    Debug.Log("Boss performs Claw Swipe.");
                    PerformClawSwipe();
                    yield return new WaitForSeconds(1.5f); // Adjust timing as needed
                    break;

                case BossAction.TailSweep:
                    Debug.Log("Boss performs Tail Sweep.");
                    PerformTailSweep();
                    yield return new WaitForSeconds(1.5f); // Adjust timing as needed
                    break;

                case BossAction.SummonMinions:
                    Debug.Log("Boss summons minions.");
                    PerformSummonMinions();
                    yield return new WaitForSeconds(2f); // Adjust timing as needed
                    break;

                case BossAction.KnockBack:
                    Debug.Log("Boss performs an idle animation.");
                    PlayIdleAnimation();
                    yield return new WaitForSeconds(1f); // Adjust timing as needed
                    break;
            }
        }

        isExecutingCombo = false; // Combo execution is complete
    }



    /// <summary>
    /// Gets a non-repeating melee attack index.
    /// </summary>
    /// <returns>An index for the next melee attack.</returns>
    private int GetNonRepeatingMeleeAttackIndex()
    {
        int totalMeleeAttacks = 4; // Number of melee attacks (0 to 5)
        int newMeleeAttackIndex;

        do
        {
            newMeleeAttackIndex = Random.Range(0, totalMeleeAttacks);
        }
        while (newMeleeAttackIndex == lastMeleeAttackIndex);

        lastMeleeAttackIndex = newMeleeAttackIndex; // Update the last attack index
        return newMeleeAttackIndex;
    }



    /// <summary>
    /// Plays the idle animation.
    /// </summary>
    private void PlayIdleAnimation()
    {
        Debug.Log("Dragon performs a menacing idle animation.");

        animator.SetTrigger("KnockBackRoar");

    }

    private bool CanPerformFireBreath()
    {
        // Check if the player is in the range sensor
        return RangeSensor.IsPlayerInRange(); // Replace with your RangeSensor logic
    }

    private void PerformFireBreath()
    {
        Debug.Log("Dragon uses Fire Breath!");
        animator.SetTrigger("FireBreath");
        if (fireBreathEffect != null) fireBreathEffect.Play();

    }

    private bool CanPerformClawSwipe()
    {
        // Check if the player is in melee range and in front of the dragon
        return MeleeSensor.IsPlayerInRange() && IsPlayerInFront();
    }

    private void PerformClawSwipe()
    {
        Debug.Log("Dragon performs Claw Swipe!");

        animator.SetTrigger("ClawSwipe");
    }

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < 45f; // Adjust angle threshold as needed
    }


    private bool CanPerformTailSweep()
    {
        // Check if the player is in melee range and behind the dragon
        return MeleeSensor.IsPlayerInRange() && IsPlayerBehind();
    }

    private void PerformTailSweep()
    {
        Debug.Log("Dragon performs Tail Sweep!");

        animator.SetTrigger("TailSweep");
    }


    private bool IsPlayerBehind()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle > 135f; // Adjust angle threshold as needed
    }

    private bool CanPerformSummonMinions()
    {
        // Check if the player is out of melee range
        return !MeleeSensor.IsPlayerInRange();
    }

    private void PerformSummonMinions()
    {
        Debug.Log("Dragon summons minions!");

        animator.SetTrigger("SummonMinions");

    }

    /// <summary>
    /// Triggers an attack in Phase 2.
    /// </summary>
    private void AttackPhase2()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= phase2AttackCooldown)
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
                if (movementController) movementController.enabled = true; // Enable movement controller
                break;

            case BossPhase.Phase2:
                Debug.Log("Boss transitioned to Phase 2: Aerial.");
                if (movementController) movementController.enabled = false; // Disable grounded movement logic
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
        if (movementController) movementController.enabled = false;
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

    private void LockMovement()
    {
        //Debug.Log("Movement locked.");
        if (movementController) movementController.LockMovement(); // Call LockMovement from BossRotationWithAnimation
    }

    private void UnlockMovement()
    {
        //Debug.Log("Movement unlocked.");
        if (movementController) movementController.UnlockMovement(); // Call UnlockMovement from BossRotationWithAnimation
    }

}
