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
        Melee2Hit01,
        Melee3Hit01,
        Melee2Hit02,
        MeleeRollAttack01,
        KickAttack,
        OneHandCast01,
        TwoHandCast01,
        TwoHandCast02,
        TwoHandCasr03,
        EnRage,
        Laser,
        OffmapCast01
    }

    private List<BossAction> MeleeCombo01 = new List<BossAction> { BossAction.Melee2Hit01};
    private List<BossAction> MeleeCombo02 = new List<BossAction> { BossAction.Melee3Hit01};
    private List<BossAction> MeleeCombo03 = new List<BossAction> { BossAction.Melee2Hit02};
    private List<BossAction> RangeCombo01 = new List<BossAction> { BossAction.OneHandCast01};   //Cast Basic
    private List<BossAction> OutRangeCombo01 = new List<BossAction> { BossAction.KickAttack};   //Kick Enemy
    private List<BossAction> SpecialCombo01 = new List<BossAction> { BossAction.EnRage};        //Call Enemy
    private List<BossAction> SpecialCombo02 = new List<BossAction> { BossAction.Laser};         //Call laser
    private List<BossAction> SpecialCombo03 = new List<BossAction> { BossAction.OffmapCast01};   //OffmapCast around boss


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

    private float attackTimer = 0f;
    [SerializeField] private int comboCounter = 0;

    [Header("Phase 1 Settings")]
    [SerializeField] private float phase1AttackCooldown;

    [Header("Phase 2 Settings")]
    [SerializeField] private float phase2AttackCooldown;
    
    [Header("Enrage Settings")]
    [SerializeField] private bool isEnraged = false;
    [SerializeField] private bool enableEnrage = true;
    [SerializeField] private float enrageThreshold = 0.5f; 
    


    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
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
        //Debug.Log(combo);
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
            Debug.Log("Executing action: " + action);
            switch (action)
            {
                case BossAction.Melee2Hit01:
                    BossAnimation.PerformAttack01();
                    yield return new WaitForSeconds(5f);
                    break;
                
                case BossAction.Melee3Hit01:
                    BossAnimation.PerformAttack02();
                    yield return new WaitForSeconds(5f);
                    break;

                case BossAction.Melee2Hit02:
                    BossAnimation.PerformAttack03();
                    yield return new WaitForSeconds(5f); 
                    break;

                case BossAction.MeleeRollAttack01:
                    BossAnimation.PerformAttack04(); 
                    yield return new WaitForSeconds(5f); 
                    break;
                
                case BossAction.KickAttack:
                    BossAnimation.PerformAttack05(); 
                    yield return new WaitForSeconds(3f); 
                    break;

                case BossAction.OneHandCast01:
                    BossAnimation.PerformCast05(); 
                    yield return new WaitForSeconds(3f); 
                    break;

                case BossAction.EnRage:
                    BossAnimation.PerformCast01(); 
                    yield return new WaitForSeconds(3f); 
                    break;

                case BossAction.Laser:
                    BossAnimation.PerformCast02(); 
                    yield return new WaitForSeconds(3.5f); 
                    break;

                case BossAction.OffmapCast01:
                    BossAnimation.PerformCast06(); 
                    yield return new WaitForSeconds(3f); 
                    break;
        }

        comboCounter++;
        isExecutingCombo = false;
        BossAnimation.UnlockMovement();
        }
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
            if (comboCounter >= 5)
            {
                float randomChance = Random.value;

                if(randomChance <= 0.3f){
                    StartCombo(SpecialCombo01);
                }else if (randomChance <= 0.6f){
                    StartCombo(SpecialCombo02);
                }else{
                    StartCombo(SpecialCombo03);
                }
                comboCounter = 0;
                return;
            }

            // Decide which combo to execute based on player position
            if (rangeSensor.IsPlayerInRange() && rangeSensor.IsPlayerInFront() && !meleeSensor.IsPlayerInRange()) //IN RANGE
            {
                // Randomly choose between range combos
                float randomChance = Random.value;

                if(randomChance <= 1f){
                    StartCombo(RangeCombo01); 
                }
            }
            else if (meleeSensor.IsPlayerInRange() && meleeSensor.IsPlayerInFront()) //IN MELEE
            {
                float randomChance = Random.value;

                if(randomChance <= 1f){
                    StartCombo(MeleeCombo01); 
                }
            }
            else if (rangeSensor.IsPlayerOutOfRange() && rangeSensor.IsPlayerInFront()) //OUT RANGE
            {
                float randomChance = Random.value;

                if(randomChance <= 1f){
                    StartCombo(OutRangeCombo01); 
                    return;
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
