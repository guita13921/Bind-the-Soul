using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int deathCount = 0;

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

    /// <summary>
    /// ///////////
    /// </summary>/
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

    public int QSkillType = 0;

    public bool Q1_QKDamageUp = false;
    public bool Q1_QKFasterWider = false;
    public bool Q1_QKKillEnemyDamageUp = false;

    public bool Q2_QKCrit = false;
    public bool Q2_QKStackable = false;
    public bool Q2_SmallBullet = false;

    public bool Q3_QKWeak = false;
    public bool Q3_QKExplode = false;
    public bool Q3_Barrier = false;

    public void ResetToDefault()
    {
        deathCount = 0;

        vampirism = 0;
        barrierLV = 0;

        forthNormalAttack = false;
        normalAttackDamageUpLV = 0;
        normalAttackSlash = false;

        specialAttack = 0; // Default to "spin"
        specialAdd1 = false;
        specialAdd2 = false;
        specialAdd3 = false;
        specialAdd4 = false;
        specialAdd5 = false;
        specialAdd6 = false;

        specialBiggerLV = 0;
        SpecialDamageUpLV = 0;

        qskillName = "normal"; // Default skill
        skillDamageUpLV = 0;
        skillSlowEnemyLV = 0;
        skillPoisionEnemyLV = 0;
    }
}
