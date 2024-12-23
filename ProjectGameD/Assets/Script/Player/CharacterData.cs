using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Normal Attack")]
    public bool forthNormalAttack = false; // add the forth attack to MC
    public bool normalAttackSlash = false; //MC third attack has a slash forward

    [Space]
    //Speacial Attack buff
    [Header("Special Attack")]
    public int specialAttack = 1; //0 for spin ,1 for big sword , 2 for red spin

    [Space]
    public bool specialAdd1 = false; // make special bigger

    public bool specialAdd2 = false; //

    [Header("Skill name")]
    public string qskillName = "normal"; // lightning    [Space]
}
