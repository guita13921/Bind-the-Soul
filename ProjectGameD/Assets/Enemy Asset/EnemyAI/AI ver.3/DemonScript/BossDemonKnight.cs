using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class DemonKnightBoss : MonoBehaviour
{

    public enum BossPhase
    {
        Phase1,  // Grounded phase
        Phase2,  // Aerial phase
        Dead
    }

    private enum BossAction
    {
        Attack,
        SpecialAttack
    }

    [SerializeField] private List<BossAction> MeleeCombo01 = new List<BossAction> { BossAction.Attack};
    [SerializeField] private List<BossAction> RangeCombo01 = new List<BossAction> { BossAction.SpecialAttack};
    [SerializeField] private List<BossAction> OutRangeCombo01 = new List<BossAction> { BossAction.SpecialAttack};


    private List<BossAction> currentCombo = new List<BossAction>(); // Stores the current combo
    public bool isExecutingCombo = false; // Tracks if a combo is currently being executed

    [SerializeField] private BossPhase currentPhase = BossPhase.Phase1;

    [Header("Reference")]
    [SerializeField] public BossDemon_Animation BossAnimation;
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] public RangeSensor rangeSensor;
    [SerializeField] public MeleeSensor meleeSensor;
    public Transform player;
    public NavMeshAgent agent;
    private Animator animator;

     private float attackTimer = 0f;

    [Header("Phase 1 Settings")]
    [SerializeField] private float phase1AttackCooldown = 2f;

    [Header("Phase 2 Settings")]
    [SerializeField] private float phase2AttackCooldown = 1f;
    
    [Header("Enrage Settings")]
    [SerializeField] private bool isEnraged = false;
    [SerializeField] private bool enableEnrage = true;
    [SerializeField] private float enrageThreshold = 0.5f; 
    


    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                HandlePhase1();
                break;

            case BossPhase.Phase2:
                //HandlePhase2();
                break;

            case BossPhase.Dead:
                break;
        }
        
        if (enemyHealth.GetCurrentHealth() <= 0 && currentPhase != BossPhase.Dead)
        {
            TransitionToPhase(BossPhase.Dead);
        }
        else if (enableEnrage && !isEnraged && enemyHealth.GetCurrentHealth() <= enemyHealth.GetMaxHealth() * enrageThreshold)
        {
            //TransitionToPhase(BossPhase.Enraged);
        }
    }

    private void StartCombo(List<BossAction> combo)
    {
        if (isExecutingCombo) return;
        currentCombo = combo;
        StartCoroutine(ExecuteCombo());
    }

    private IEnumerator ExecuteCombo()
    {
        isExecutingCombo = true;
        BossAnimation.LockMovement();

        foreach (BossAction action in currentCombo)
        {
            switch (action)
            {
                case BossAction.Attack:
                    //Debug.Log("Boss moves backward.");
                    BossAnimation.PerformAttack();
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;
                
                case BossAction.SpecialAttack:
                    //Debug.Log("Boss moves ForwardRush.");
                    BossAnimation.PerformSpecialAttack(); // Custom ForwardRush logic
                    yield return new WaitForSeconds(3f); // Adjust timing as needed
                    break;
            }
        }

        isExecutingCombo = false;
        BossAnimation.UnlockMovement();
    }

    private void HandlePhase1()
    {
        if (player == null) return;

        AttackPhase1();
    }

    private void AttackPhase1()
    {
        attackTimer += Time.deltaTime;

        // Perform an attack only after the cooldown
        if (attackTimer >= phase1AttackCooldown && !isExecutingCombo)
        {
            attackTimer = 0f;

            // Decide which combo to execute based on player position
            if (rangeSensor.IsPlayerInRange() && !meleeSensor.IsPlayerInRange() && meleeSensor.IsPlayerInFront())
            {
                // Randomly choose between range combos
                float randomChance = Random.value;
                if(randomChance <= 1f){
                    StartCombo(MeleeCombo01); 
                }
            }
            else if (meleeSensor.IsPlayerInRange())
            {
                float randomChance = Random.value;
                if(randomChance <= 1f){
                    StartCombo(RangeCombo01); 
                }
            }
            else if (rangeSensor.IsPlayerOutOfRange() && meleeSensor.IsPlayerInFront())
            {
                float randomChance = Random.value;
                if(randomChance <= 1f){
                    StartCombo(OutRangeCombo01); 
                }
            }
        }
    }

private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;

        switch (newPhase)
        {
            case BossPhase.Dead:
                HandleDeath();
                break;
        }
    }


    private void HandleDeath()
    {
        Debug.Log("Boss has been defeated!");
        //animator.SetTrigger("Death");
        //if (movementController) movementController.enabled = false;
        enabled = false;
        Destroy(gameObject, 5f);
    }
}
