using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int deathCount = 0;
    public int rerollpoint = 1;
    public float Health = 0;
    public float maxHealth = 1000;
    public float healthRatio = 1;

    /*
        [Header("Passive")]
        public int barrierLV = 0;
    
        [Space]
        [Header("Normal Attack")]
        public bool forthNormalAttack = false; // add the forth attack to MC
        public int normalAttackDamageUpLV = 0;
        public bool normalAttackSlash = false;
    
        [Space]
        //Speacial Attack buff
        [Header("Special Attack")]
        public int specialAttack = 0; //0 for spin ,1 for big sword , 2 for red spin,3 for heaven
    
        [Space]
        public bool specialAdd1 = false; //
    
        public bool specialAdd2 = false; //
        public bool specialAdd3 = false; //
        public bool specialAdd4 = false; //
        public bool specialAdd5 = false; //
        public bool specialAdd6 = false; //
    
        public int specialBiggerLV = 0;
        public int SpecialDamageUpLV = 0;
    
        [Header("Skill name")]
        public string qskillName = "normal"; // lightning    [Space]
        public int skillDamageUpLV = 0;
        public int skillSlowEnemyLV = 0;
        public int skillPoisionEnemyLV = 0;
    */
    // /// <summary>
    // /// ///////////
    // /// </summary>/
    [Header("New System")]
    public int reduceIncomeDamage = 0;
    public int specialLV = 0;
    public int healToThreshold = 0;
    public int QKReduceCooldown = 0;
    public int maxHPIncrease = 0;
    public int vampirism = 0;

    public int normalAttackCrit = 0;
    public int moveFaster = 0;
    public int ReduceDashCooldown = 0;
    public int reduceIncomeDamageDependOnHP = 0;
    public int addDamageDependOnHP = 0;
    public int QSkillType = 0;

    public bool Q1_QKDamageUp = false;
    public bool Q1_QKFasterWider = false;
    public bool Q1_QKKillEnemyDamageUp = false;

    public bool Q2_QKCrit = false;
    public bool Q2_QKStackable = false;
    public bool Q2_SmallBullet = false;

    public bool Q3_QKWeak = false;
    public bool Q3_QKSlow = false;
    public bool Q3_Barrier = false;

    public Health MC_health;
    public ControlPower controlPower;

    public void HpCalculation()
    {
        MC_health = FindObjectOfType<Health>();
        maxHPIncrease++;

        maxHealth += 1000 * maxHPIncrease;
        MC_health.maxHealth = maxHealth;
        MC_health.currentHealth = maxHealth * healthRatio;
    }

    public void SpeedUpgrade()
    {
        controlPower = FindObjectOfType<ControlPower>();

        moveFaster++;
        controlPower.CheckSpeed();
    }

    public void QKCooldownReduce()
    {
        controlPower = FindObjectOfType<ControlPower>();

        QKReduceCooldown++;
        controlPower.CheckQKCooldown();

    }



    public void WaitDashtimeReduce()
    {
        controlPower = FindObjectOfType<ControlPower>();
        ReduceDashCooldown++;
        controlPower.CheckWaitDashtime();

    }


    public void ResetToDefault()
    {
        maxHealth = 1000;
        Health = maxHealth;

        reduceIncomeDamage = 0;
        specialLV = 0;
        healToThreshold = 0;
        QKReduceCooldown = 0;
        maxHPIncrease = 0;
        vampirism = 0;
        normalAttackCrit = 0;
        moveFaster = 0;
        ReduceDashCooldown = 0;
        reduceIncomeDamageDependOnHP = 0;
        QSkillType = 0;
        addDamageDependOnHP = 0;

        Q1_QKDamageUp = false;
        Q1_QKFasterWider = false;
        Q1_QKKillEnemyDamageUp = false;
        Q2_QKCrit = false;
        Q2_QKStackable = false;
        Q2_SmallBullet = false;
        Q3_QKWeak = false;
        Q3_QKSlow = false;
        Q3_Barrier = false;
        rerollpoint = 1;
        healthRatio = 1;
    }


}
