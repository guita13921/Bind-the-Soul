using System.Collections;
using System.Collections.Generic; // This is required for List<>
using UnityEngine;
using UnityEngine.AI;

public class DragonBoss : MonoBehaviour
{
    // Enum for boss phases
    [SerializeField] public enum BossPhase
    {
        Phase1,  // Grounded phase
        Phase2,  // Aerial phase
        Enraged, // Optional enraged state
        Dead
    }

    // Current phase of the boss
    [SerializeField] private BossPhase currentPhase = BossPhase.Phase1;

    [Header("General Settings")]
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] private NavMeshAgent agent;    // NavMeshAgent for movement

    [SerializeField] private Transform player; // Reference to the player

    [Header("Phase 1 Settings")]
    [SerializeField] private float phase1AttackCooldown = 6f;

    [Header("Phase 2 Settings")]
    [SerializeField] private float phase2AttackCooldown = 10f;
    [SerializeField] private float phase2Height = 10f; // Height for flying phase
    [SerializeField] private float flySpeed = 5f;     // Movement speed during Phase 2
    private bool isFlying = false; // Whether the boss is in the air

    [Header("Enrage Settings")]
    [SerializeField] private bool enableEnrage = true;
    [SerializeField] private float enrageThreshold = 0.25f; // Health percentage to trigger enraged state

    private bool isEnraged = false;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DragonBossAnimations bossAnimations;
    [SerializeField] private DragonBossFlight dragonBossFlight;
    private BossRotationWithAnimation movementController;

    [Header("Sensor")]
    [SerializeField] private RangeSensor RangeSensor;
    [SerializeField] private MeleeSensor MeleeSensor;
    private float attackTimer = 0f;
    

    private enum BossAction
    {
        Backward,
        ForwardRush,
        FireBreath,
        Laser,
        ClawSwipe,
        TailSweep,
        SummonMinions,
        KnockBack,
        FireRain,
        DiveBomb,
        LavaPits
    }
    

    private List<BossAction> currentCombo = new List<BossAction>(); // Stores the current combo
    public bool isExecutingCombo = false; // Tracks if a combo is currently being executed

    [Header("Agent")]
    //[SerializeField] private List<BossAction> TEST = new List<BossAction> { BossAction.Backward, BossAction.ForwardRush };
    //Range
    //FireVar01 fire 50%
    [SerializeField] private List<BossAction> Rangecombo1 = new List<BossAction> { BossAction.Backward, BossAction.FireBreath };
    //FireVar02 laser 25%
    [SerializeField] private List<BossAction> Rangecombo2 = new List<BossAction> { BossAction.KnockBack, BossAction.Backward, BossAction.Laser };
    
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
    //20% rush dash
    [SerializeField] private List<BossAction> OutRangecombo1 = new List<BossAction> { BossAction.ForwardRush, BossAction.TailSweep};
    //20% summon
    [SerializeField] private List<BossAction> OutRangecombo2 = new List<BossAction> { BossAction.SummonMinions};
    //60% Laser
    [SerializeField] private List<BossAction> OutRangecombo3 = new List<BossAction> { BossAction.Laser};

    //Phase2Fly
    [SerializeField] private List<BossAction> Phase2Fly01 = new List<BossAction> { BossAction.FireRain};
    [SerializeField] private List<BossAction> Phase2Fly02 = new List<BossAction> { BossAction.LavaPits};
    [SerializeField] private List<BossAction> Phase2Fly03 = new List<BossAction> { BossAction.DiveBomb};
    private void Start()
    {
        // Ensure references
        if (!player) player = GameObject.FindWithTag("Player").transform;

        movementController = GetComponent<BossRotationWithAnimation>();
        if (!movementController) Debug.LogError("BossRotationWithAnimation script is missing!");

        TransitionToPhase(BossPhase.Phase1);
        
    }

    public void Update()
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
        
        if (enemyHealth.GetCurrentHealth() <= 0 && currentPhase != BossPhase.Dead)
        {
            TransitionToPhase(BossPhase.Dead);
        }
        else if (enableEnrage && !isEnraged && enemyHealth.GetCurrentHealth() <= enemyHealth.GetMaxHealth() * enrageThreshold)
        {
            TransitionToPhase(BossPhase.Enraged);
        }
    }

    /// Handles the behavior for Phase 1.
    private void HandlePhase1()
    {
        if (player == null) return;

        AttackPhase1();
    }

    /// Handles the behavior for Phase 2.
    private void HandlePhase2()
    {
        if (player == null) return;
        AttackPhase2();
    }

    /// Handles the behavior for the enraged phase.
    private void HandleEnraged()
    {
        //Debug.Log("Boss is enraged! Increased aggression!");
        phase2AttackCooldown *= 0.75f;
        if (currentPhase == BossPhase.Phase2) HandlePhase2();
    }

    private bool IsPlayerInFront()
    {
        if (player == null) return false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer <= 45f; // Adjust the angle as needed
    }

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
                    StartCombo(Rangecombo1); // Fire Breath
                    //StartCombo(TEST); // Backward + Forward Rush
                }
                else
                {
                    StartCombo(Rangecombo2); //
                    //StartCombo(TEST); // Backward + Forward Rush
                }
            }
            else if (MeleeSensor.IsPlayerInRange())
            {
                float randomChance = Random.value; // Random value between 0.0 and 1.0

                if (randomChance < 0.35f) // 35% chance
                {
                    StartCombo(Maleecombo1); // Tail Sweep + Claw Swipe
                    //StartCombo(TEST); // Backward + Forward Rush
                }
                else if (randomChance < 0.70f) // Another 35% chance (total 70%)
                {
                    StartCombo(Maleecombo2); // Claw Swipe + Tail Sweep
                    //StartCombo(TEST); // Backward + Forward Rush
                }
                else if (randomChance < 0.90f) // 20% chance (total 90%)
                {
                    StartCombo(Maleecombo3); // Backward + Forward Rush
                }
                else // Remaining 10% chance
                {
                    StartCombo(Maleecombo4); // Knockback (Evade)
                    //StartCombo(TEST); // Backward + Forward Rush
                }
            }
            else if (RangeSensor.IsPlayerOutOfRange() && IsPlayerInFront())
            {
                // Randomly choose between out-of-range combos
                float randomChance = Random.value;
                if (randomChance < 0.2f)
                {
                    StartCombo(OutRangecombo1); // Forward Rush + Claw Swipe
                }else if (randomChance < 0.4f)
                {
                    StartCombo(OutRangecombo2); // Backward + Forward Rush
                }else 
                {
                    StartCombo(OutRangecombo3); // Knockback (Evade)
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
                    //Debug.Log("Boss moves backward.");
                    animator.SetTrigger("DashBackward");
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;
                
                case BossAction.ForwardRush:
                    //Debug.Log("Boss moves ForwardRush.");
                    bossAnimations.PerformForwardRush(); // Custom ForwardRush logic
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;

                case BossAction.FireBreath:
                    //Debug.Log("Boss performs Fire Breath.");
                    bossAnimations.PerformFireBreath(); // Custom fire breath logic
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.Laser:
                    //Debug.Log("Boss fires a laser.");
                    bossAnimations.PerformLaser(); // Custom fire breath logic
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.ClawSwipe:
                    //Debug.Log("Boss performs Claw Swipe.");
                    bossAnimations.PerformClawSwipe();
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;

                case BossAction.TailSweep:
                    //Debug.Log("Boss performs Tail Sweep.");
                    bossAnimations.PerformTailSweep();
                    yield return new WaitForSeconds(2.5f); // Adjust timing as needed
                    break;

                case BossAction.SummonMinions:
                    //Debug.Log("Boss summons minions.");
                    bossAnimations.PerformSummonMinions();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;

                case BossAction.KnockBack:
                    //Debug.Log("Boss performs an KnockBack animation.");
                    bossAnimations.PerformKnockBack();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;
                case BossAction.FireRain:
                    Debug.Log("Boss performs an FireRain.");
                    dragonBossFlight.PerformFireTornado();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;
                case BossAction.DiveBomb:
                    Debug.Log("Boss performs an DiveBomb.");
                    dragonBossFlight.PerformFireTornado();
                    yield return new WaitForSeconds(6f); // Adjust timing as needed
                    break;
                case BossAction.LavaPits:
                    Debug.Log("Boss performs an FireTornado.");
                    dragonBossFlight.PerformFireTornado();
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
        if (!isFlying)
        {
            StartFlying();
        }else{
            FlyTowardPlayer();
            float randomChance = Random.value;
            if (randomChance < 0.35f)
            {
                StartCombo(Phase2Fly01);
            }else if (randomChance < 0.7f)
            {
                StartCombo(Phase2Fly02); 
            }else 
            {
                StartCombo(Phase2Fly03);
            }
        }

    }

    /// Transitions the boss to a new phase.
    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;

        switch (newPhase)
        {
            case BossPhase.Phase1:
                //Debug.Log("Boss transitioned to Phase 1: Grounded.");
                if (movementController) movementController.enabled = true; // Enable movement controller
                break;

            case BossPhase.Phase2:
                //Debug.Log("Boss transitioned to Phase 2: Aerial.");
                if (movementController) movementController.enabled = false; // Disable grounded movement logic
                break;

            case BossPhase.Enraged:
                //Debug.Log("Boss is now Enraged!");
                isEnraged = true;
                bossAnimations.TriggerEnrage();
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

    private void StartFlying()
    {
        isFlying = true;
        agent.enabled = true;
        animator.SetTrigger("FlyRoam"); // Play the "fly roam" animation
    }

    private void FlyTowardPlayer()
    {
        if (player == null) return;

        // Move the boss toward the player position while maintaining flying height
        Vector3 targetPosition = new Vector3(player.position.x, phase2Height, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flySpeed * Time.deltaTime);

        // Rotate the boss smoothly toward the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    }

    public void EndEnrage(){
        TransitionToPhase(BossPhase.Phase2);
    }
}
