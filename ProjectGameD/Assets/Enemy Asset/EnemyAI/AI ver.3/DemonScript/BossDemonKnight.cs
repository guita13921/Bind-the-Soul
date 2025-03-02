using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonKnightBoss : MonoBehaviour
{

    public enum BossPhase
    {
        Phase1,
        Phase1_Enraged,
        Phase2, 
        Phase2_Enraged,
        Dead
    }

    private enum BossAction
    {
        Melee2Hit01,
        Melee3Hit01,
        Melee2Hit02,
        MeleeRollAttack01,
        Dashing,
        KickAttack,
        OneHandCast01,
        TwoHandCast01,
        TwoHandCast02,
        TwoHandCasr03,
        CallEnemy,
        Laser,
        OffmapCast01
    }

    //MALEE COMBO
    private List<BossAction> MeleeCombo01 = new List<BossAction> { BossAction.Melee2Hit01};
    private List<BossAction> MeleeCombo02 = new List<BossAction> { BossAction.Melee3Hit01};
    private List<BossAction> MeleeCombo03 = new List<BossAction> { BossAction.Melee2Hit02};
    private List<BossAction> MeleeCombo04 = new List<BossAction> { BossAction.MeleeRollAttack01};
    
    //RANGE COMBO
    private List<BossAction> RangeCombo01 = new List<BossAction> { BossAction.OneHandCast01};   //Cast Basic
    private List<BossAction> RangeCombo02 = new List<BossAction> { BossAction.Dashing,BossAction.Melee2Hit01 };
    private List<BossAction> RangeCombo03 = new List<BossAction> { BossAction.Dashing,BossAction.Melee2Hit02};
    private List<BossAction> RangeCombo04 = new List<BossAction> { BossAction.Dashing,BossAction.MeleeRollAttack01 };
    private List<BossAction> RangeCombo05 = new List<BossAction> { BossAction.Dashing,BossAction.Melee2Hit01, BossAction.OneHandCast01};
    private List<BossAction> RangeCombo06 = new List<BossAction> { BossAction.Dashing,BossAction.Melee2Hit02, BossAction.OneHandCast01};

    //OUT-RANGE COMBO
    private List<BossAction> OutRangeCombo01 = new List<BossAction> { BossAction.Dashing, BossAction.OneHandCast01}; 
    private List<BossAction> OutRangeCombo02 = new List<BossAction> { BossAction.OneHandCast01, BossAction.OneHandCast01};
    private List<BossAction> OutRangeCombo03 = new List<BossAction> { BossAction.KickAttack, BossAction.OneHandCast01}; 
    private List<BossAction> OutRangeCombo04 = new List<BossAction> { BossAction.KickAttack, BossAction.Dashing, BossAction.Melee3Hit01 }; 

    //SPECIAL COMBO
    private List<BossAction> SpecialCombo01 = new List<BossAction> { BossAction.CallEnemy};   
    private List<BossAction> SpecialCombo02 = new List<BossAction> { BossAction.Laser};      
    private List<BossAction> SpecialCombo03 = new List<BossAction> { BossAction.OffmapCast01};  


    private List<BossAction> currentCombo = new List<BossAction>(); // Stores the current combo
    public bool isExecutingCombo = false; // Tracks if a combo is currently being executed

    [SerializeField] private BossPhase currentPhase = BossPhase.Phase1;

    [Header("Reference")]
    [SerializeField] public BossDemon_Animation BossAnimation;
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] public RangeSensor rangeSensor;
    [SerializeField] public MeleeSensor meleeSensor;
    [SerializeField] public BossDemon_Rotation bossDemon_Rotation;
    public Transform player;
    public NavMeshAgent agent;

    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private int comboCounter = 0;

    [Header("Phase 1 Settings")]
    [SerializeField] private float phase1AttackCooldown;
    [SerializeField] private float phase1AttackCooldown_Enrage;

    [Header("Phase 2 Settings")]
    [SerializeField] private float phase2AttackCooldown;
    [SerializeField] private float phase2AttackCooldown_Enrage;

    [Header("EnRage")]
    private bool hasRevived = false; // Tracks if the boss has revived before
    [SerializeField] private GameObject Aura;
    [SerializeField] private bool isEnrage = false;
    private float maleeTimeusing = 4f;
    private float MaleeTimeRage = 1f;

    [Header("HitBox")]
    [SerializeField] private CapsuleCollider bodyHitBox;
    
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (enemyHealth.GetCurrentHealth() <= 0)
        {
            if (!hasRevived)
            {
                // First-time death: Revive and transition to Phase 2
                hasRevived = true;
                TransitionToPhase(BossPhase.Phase2);
                enemyHealth.RestoreFullHealth(); // Assuming you have a function to restore health
                return;
            }

            // If already revived once, proceed to death phase
            if (currentPhase == BossPhase.Phase2 || currentPhase == BossPhase.Phase2_Enraged)
            {
                TransitionToPhase(BossPhase.Dead);
            }
            return;
        }

        switch (currentPhase)
        {
            case BossPhase.Phase1:
                HandlePhase1();
                if (enemyHealth.GetCurrentHealth() <= enemyHealth.GetMaxHealth() * 0.5f)
                {
                    TransitionToPhase(BossPhase.Phase1_Enraged);
                }
                break;

            case BossPhase.Phase1_Enraged:
                HandlePhase1Enraged();
                break;

            case BossPhase.Phase2:
                HandlePhase2();

                if (enemyHealth.GetCurrentHealth() <= enemyHealth.GetMaxHealth() * 0.5f)
                {
                    TransitionToPhase(BossPhase.Phase2_Enraged);
                    maleeTimeusing = MaleeTimeRage;
                }
                break;

            case BossPhase.Phase2_Enraged:
                HandlePhase2Enraged();
                break;
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
                    yield return new WaitForSeconds(maleeTimeusing); //Time must == Animation Time
                    break;
                
                case BossAction.Melee3Hit01:
                    BossAnimation.PerformAttack02();
                    yield return new WaitForSeconds(maleeTimeusing);
                    break;

                case BossAction.Melee2Hit02:
                    BossAnimation.PerformAttack03();
                    yield return new WaitForSeconds(maleeTimeusing); 
                    break;

                case BossAction.MeleeRollAttack01:
                    BossAnimation.PerformAttack04(); 
                    yield return new WaitForSeconds(maleeTimeusing); 
                    break;
                
                case BossAction.KickAttack:
                    BossAnimation.PerformAttack05(); 
                    yield return new WaitForSeconds(2f); 
                    break;

                case BossAction.OneHandCast01:
                    BossAnimation.PerformCast05(); 
                    yield return new WaitForSeconds(3f); 
                    break;


                case BossAction.Laser:
                    BossAnimation.PerformCast02(); 
                    yield return new WaitForSeconds(10f); 
                    BossAnimation.StopLaser();
                    break;

                case BossAction.OffmapCast01:
                    BossAnimation.PerformCast06(); 
                    yield return new WaitForSeconds(6f);  
                    break;

                case BossAction.CallEnemy:
                    BossAnimation.PerformSummonMinions(); 
                    yield return new WaitForSeconds(6f);  
                    break;
                
                case BossAction.Dashing:
                    BossAnimation.StartDashing(); 
                    yield return new WaitForSeconds(0.1f);  
                    break;
        }

            if(action != BossAction.Dashing){
                comboCounter++;
                isExecutingCombo = false;
                BossAnimation.UnlockMovement();
            }
        }
    }

    private void HandlePhase1()
    {
        if (player == null) return;
        AttackPhase1();
    }

    private void HandlePhase1Enraged()
    {
        if (player == null) return;
        AttackPhase1_EnRage();
    }

    private void HandlePhase2()
    {
        if (player == null) return;
        AttackPhase2();
    }

    private void HandlePhase2Enraged()
    {
        if (player == null) return;
        AttackPhase2_EnRage();
    }

    private void AttackPhase1()
    {
        attackTimer += Time.deltaTime;

        // Perform an attack only after the cooldown
        if (attackTimer >= phase1AttackCooldown && !isExecutingCombo)
        {
            
            if (comboCounter >= 5)
            {
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance < 0.4f){
                    StartCombo(SpecialCombo01);
                }else if (randomChance < 0.8f){
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
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance < 0.25f){
                    StartCombo(RangeCombo02); 
                }else if(randomChance < 0.5f){
                    StartCombo(RangeCombo03);
                }else{
                    StartCombo(RangeCombo01);
                }
            }
            else if (meleeSensor.IsPlayerInRange() && meleeSensor.IsPlayerInFront()) //IN MELEE
            {
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance <= 0.5f){
                    StartCombo(MeleeCombo01); 
                }else if(randomChance <= 0.8f){
                    StartCombo(MeleeCombo03); 
                }else{
                    StartCombo(MeleeCombo04);     
                }
            }
            else if (rangeSensor.IsPlayerOutOfRange() && rangeSensor.IsPlayerInFront()) //OUT RANGE
            {
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance <= 0.5f){
                    StartCombo(OutRangeCombo01); 
                }else{
                    StartCombo(OutRangeCombo02); 
                }

            }else{
                bossDemon_Rotation.RequestInsideLookAtPlayer();
            }
        }
    }

    private void AttackPhase1_EnRage()
    {
        attackTimer += Time.deltaTime;

        // Perform an attack only after the cooldown
        if (attackTimer >= phase1AttackCooldown_Enrage && !isExecutingCombo)
        {
            //attackTimer = 0f;
            if (comboCounter >= 4)
            {
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance < 0.4f){
                    StartCombo(SpecialCombo01);
                }else if (randomChance < 0.8f){
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
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance < 0.50f){
                    StartCombo(RangeCombo02); 
                }else if(randomChance < 0.75f){
                    StartCombo(RangeCombo03);
                }else{
                    StartCombo(RangeCombo01);
                }
            }
            else if (meleeSensor.IsPlayerInRange() && meleeSensor.IsPlayerInFront()) //IN MELEE
            {
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance <= 0.2f){
                    StartCombo(MeleeCombo02); 
                }else if(randomChance < 0.5f){
                    StartCombo(RangeCombo04);
                }else{
                    StartCombo(MeleeCombo03); 
                }
            }
            else if (rangeSensor.IsPlayerOutOfRange() && rangeSensor.IsPlayerInFront()) //OUT RANGE
            {
                attackTimer = 0f;
                float randomChance = Random.value;

                if(randomChance <= 0.5f){
                    StartCombo(OutRangeCombo01); 
                }else{
                    StartCombo(OutRangeCombo02); 
                }

            }else{
                bossDemon_Rotation.RequestInsideLookAtPlayer();
            }
        }
    }

    private void AttackPhase2()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= phase2AttackCooldown && !isExecutingCombo)
        {
            if (comboCounter >= 4)
            {
                attackTimer = 0f;
                float randomChance = Random.value;
                comboCounter = 0;
                return;
            }

            if (rangeSensor.IsPlayerInRange() && rangeSensor.IsPlayerInFront() && !meleeSensor.IsPlayerInRange()) //IN RANGE
            {
                attackTimer = 0f;
                float randomChance = Random.value;
            }
            else if (meleeSensor.IsPlayerInRange() && meleeSensor.IsPlayerInFront()) //IN MELEE
            {
                attackTimer = 0f;
                float randomChance = Random.value;
            }
            else if (rangeSensor.IsPlayerOutOfRange() && rangeSensor.IsPlayerInFront()) //OUT RANGE
            {
                attackTimer = 0f;
                float randomChance = Random.value;

            }else{
                bossDemon_Rotation.RequestInsideLookAtPlayer();
            }
        }
    }

    private void AttackPhase2_EnRage()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= phase2AttackCooldown && !isExecutingCombo)
        {
            if (comboCounter >= 4)
            {
                attackTimer = 0f;
                float randomChance = Random.value;
                comboCounter = 0;
                return;
            }

            if (rangeSensor.IsPlayerInRange() && rangeSensor.IsPlayerInFront() && !meleeSensor.IsPlayerInRange()) //IN RANGE
            {
                attackTimer = 0f;
                float randomChance = Random.value;
            }
            else if (meleeSensor.IsPlayerInRange() && meleeSensor.IsPlayerInFront()) //IN MELEE
            {
                attackTimer = 0f;
                float randomChance = Random.value;
            }
            else if (rangeSensor.IsPlayerOutOfRange() && rangeSensor.IsPlayerInFront()) //OUT RANGE
            {
                attackTimer = 0f;
                float randomChance = Random.value;

            }else{
                bossDemon_Rotation.RequestInsideLookAtPlayer();
            }
        }
    }

    public bool GetisEnrage(){
        return isEnrage;
    }

    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;

        switch (newPhase)
        {
            case BossPhase.Phase1_Enraged:
                EnableEnrage();
                maleeTimeusing = MaleeTimeRage;
                BossAnimation.TransitionToPhase("Phase1_Enraged");
                break;
            case BossPhase.Phase2:
                DisableEnrage();
                BossAnimation.TransitionToPhase("Phase2");
                break;
            case BossPhase.Phase2_Enraged:
                EnableEnrage();
                BossAnimation.TransitionToPhase("Phase2_Enraged");
                break;            
            case BossPhase.Dead:
                HandleDeath();
                break;
        }
    }

    private void EnableEnrage(){
        isEnrage = true;
        Aura.SetActive(true);
    }

    private void DisableEnrage(){
        isEnrage = false;
        Aura.SetActive(false);
    }

    private void HandleDeath()
    {
        Debug.Log("Boss has been defeated!");
        enabled = false;
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.gameObject.CompareTag("PlayerSword"))
        {
            Debug.Log("Hit");
            attackTimer += 0.5f;
        }
    }
    
    public BossPhase GetBossPhase(){
        return currentPhase;
    }

}
