using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int deathCount = 0;

    [Header("Passive")]
    public int vampirism = 0;
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
