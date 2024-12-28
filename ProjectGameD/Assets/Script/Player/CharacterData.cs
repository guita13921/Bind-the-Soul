using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Passive")]
    public bool vampirism = false;
    public int barrierLV = 0;

    [Space]
    [Header("Normal Attack")]
    public bool forthNormalAttack = false; // add the forth attack to MC
    public bool normalAttackSlash = false; //MC third attack has a slash forward
    public int normalAttackDamageUpLV = 0;

    [Space]
    //Speacial Attack buff
    [Header("Special Attack")]
    public int specialAttack = 1; //0 for spin ,1 for big sword , 2 for red spin,for heaven

    [Space]
    public bool specialAdd1 = false; // make special bigger

    public bool specialAdd2 = false; //

    public int specialBiggerLV = 0;
    public int SpecialDamageUpLV = 0;

    [Header("Skill name")]
    public string qskillName = "normal"; // lightning    [Space]
    public int skillDamageUpLV = 0;
    public int skillSlowEnemyLV = 0;
    public int skillPoisionEnemyLV = 0;
}
