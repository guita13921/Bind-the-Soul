using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int deathCount = 0;

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

    public void ResetToDefault()
    {
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
        Q1_QKDamageUp = false;
        Q1_QKFasterWider = false;
        Q1_QKKillEnemyDamageUp = false;
        Q2_QKCrit = false;
        Q2_QKStackable = false;
        Q2_SmallBullet = false;
        Q3_QKWeak = false;
        Q3_QKSlow = false;
        Q3_Barrier = false;
    }
}
