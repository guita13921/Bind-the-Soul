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
    //[SerializeField] private ParticleSystem fireBreathEffect;
    [SerializeField] private DragonBossAnimations bossAnimations;
    private BossRotationWithAnimation movementController;

    private float attackTimer = 0f;

    [Header("Sensor")]
    [SerializeField] private RangeSensor RangeSensor;
    [SerializeField] private MeleeSensor MeleeSensor;

    private enum BossAction
    {
        Backward,
        ForwardRush,
        FireBreath,
        Laser,
        ClawSwipe,
        TailSweep,
        SummonMinions,
        KnockBack
    }

    private List<BossAction> currentCombo = new List<BossAction>(); // Stores the current combo
    public bool isExecutingCombo = false; // Tracks if a combo is currently being executed

    [Header("Agent")]
    //Range
    //FireVar01 fire 50%
    [SerializeField] private List<BossAction> RangeCombo1 = new List<BossAction> { BossAction.Backward, BossAction.FireBreath };
    //FireVar02 laser 20%
    [SerializeField] private List<BossAction> Rangecombo2 = new List<BossAction> { BossAction.KnockBack, BossAction.Backward, BossAction.Laser };
    //Summon 15%
    [SerializeField] private List<BossAction> Rangecombo3 = new List<BossAction> { BossAction.Backward, BossAction.SummonMinions };
    
    //Malee
    //Attack 35%
    [SerializeField] private List<BossAction> Maleecombo1 = new List<BossAction> { BossAction.TailSweep, BossAction.ClawSwipe };
    //Attack 35%
    [SerializeField] private List<BossAction> Maleecombo2 = new List<BossAction> { BossAction.ClawSwipe, BossAction.TailSweep };
    //Rush 20%
    [SerializeField] private List<BossAction> Maleecombo3 = new List<BossAction> { BossAction.Backward, BossAction.ForwardRush };
    //Avad 10%
    [SerializeField] private List<BossAction> Maleecombo4 = new List<BossAction> { BossAction.KnockBack};

    //Out Of range
    //50% rush dash
    [SerializeField] private List<BossAction> OutRangecombo1 = new List<BossAction> { BossAction.ForwardRush, BossAction.ClawSwipe  };
    //50% summon
    [SerializeField] private List<BossAction> OutRangecombo2 = new List<BossAction> { BossAction.SummonMinions};
    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Ensure references
        if (!player) player = GameObject.FindWithTag("Player").transform;

        movementController = GetComponent<BossRotationWithAnimation>();
        if (!movementController) Debug.LogError("BossRotationWithAnimation script is missing!");

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

    /// Handles the behavior for Phase 1.
    private void HandlePhase1()
    {
        if (player == null) return;

        // Attack logic
        AttackPhase1();
    }

    /// Handles the behavior for Phase 2.
    private void HandlePhase2()
    {
        if (player == null) return;

        // Hover in the air
        Vector3 hoverPosition = new Vector3(player.position.x, flightHeight, player.position.z);
        transform.position = Vector3.Lerp(transform.position, hoverPosition, Time.deltaTime);

        // Attack the player
        AttackPhase2();
    }

    /// Handles the behavior for the enraged phase.
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

    private bool IsPlayerInFront()
    {
        if (player == null) return false;

        // Calculate the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the angle between the boss's forward direction and the direction to the player
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check if the player is within the front cone (e.g., 90 degrees)
        return angleToPlayer <= 45f; // Adjust the angle as needed
    }

    private int lastMeleeAttackIndex = -1; // Tracks the last melee animation played

    /// Triggers an attack in Phase 1.
    private void AttackPhase1()
    {
        attackTimer += Time.deltaTime;

        // Perform an attack only after the cooldown
        if (attackTimer >= phase1AttackCooldown && !isExecutingCombo)
        {
            attackTimer = 0f;

            // Decide which combo to execute based on player position
            if (RangeSensor.IsPlayerInRange() && !MeleeSensor.IsPlayerInRange() && IsPlayerInFront())
            {
                // Randomly choose between range combos
                float randomChance = Random.value; // Random value between 0.0 and 1.0
                if (randomChance < 0.5f) // 50% chance
                {
                    StartCombo(RangeCombo1); // Fire Breath
                }
                else
                {
                    StartCombo(Rangecombo2); // Laser Attack
                }
            }
            else if (MeleeSensor.IsPlayerInRange())
            {
                // Randomly choose between melee combos
                float randomChance = Random.value; // Random value between 0.0 and 1.0
                if (randomChance < 0.5f) // 50% chance
                {
                    StartCombo(Maleecombo1); // Tail Sweep + Claw Swipe
                }
                else
                {
                    StartCombo(Maleecombo2); // Claw Swipe + Tail Sweep
                }
            }
            else if (RangeSensor.IsPlayerOutOfRange() && IsPlayerInFront())
            {
                // Randomly choose between out-of-range combos
                float randomChance = Random.value; // Random value between 0.0 and 1.0
                if (randomChance < 0.5f) // 50% chance
                {
                    StartCombo(OutRangecombo1); // Forward Rush + Claw Swipe
                }
                else
                {
                    StartCombo(OutRangecombo2); // Summon Minions
                }
            }
        }
    }


    /// Starts executing the given combo.
    private void StartCombo(List<BossAction> combo)
    {
        if (isExecutingCombo) return;
        currentCombo = combo;
        StartCoroutine(ExecuteCombo());
    }

    /// Executes the current combo step by step.
    private IEnumerator ExecuteCombo()
    {
        isExecutingCombo = true;
        bossAnimations.LockMovement();

        foreach (BossAction action in currentCombo)
        {
            switch (action)
            {
                case BossAction.Backward:
                    Debug.Log("Boss moves backward.");
                    animator.SetTrigger("DashBackward");
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;
                
                case BossAction.ForwardRush:
                    Debug.Log("Boss moves ForwardRush.");
                    bossAnimations.PerformForwardRush(); // Custom ForwardRush logic
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;

                case BossAction.FireBreath:
                    Debug.Log("Boss performs Fire Breath.");
                    bossAnimations.PerformFireBreath(); // Custom fire breath logic
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.Laser:
                    Debug.Log("Boss fires a laser.");
                    bossAnimations.PerformLaser(); // Custom fire breath logic
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.ClawSwipe:
                    Debug.Log("Boss performs Claw Swipe.");
                    bossAnimations.PerformClawSwipe();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.TailSweep:
                    Debug.Log("Boss performs Tail Sweep.");
                    bossAnimations.PerformTailSweep();
                    yield return new WaitForSeconds(2.5f); // Adjust timing as needed
                    break;

                case BossAction.SummonMinions:
                    Debug.Log("Boss summons minions.");
                    bossAnimations.PerformSummonMinions();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.KnockBack:
                    Debug.Log("Boss performs an KnockBack animation.");
                    bossAnimations.PerformKnockBack();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;
            }
        }

        isExecutingCombo = false;
        bossAnimations.UnlockMovement();
    }

    /// Triggers an attack in Phase 2.
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

    /// Transitions the boss to a new phase.
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

    /// Handles the boss's death.
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

    /// Reduces the boss's health.
    public void TakeDamage(float damage)
    {
        if (currentPhase == BossPhase.Dead) return;

        currentHealth -= damage;
        Debug.Log($"Boss Health: {currentHealth}/{maxHealth}");
    }


}
