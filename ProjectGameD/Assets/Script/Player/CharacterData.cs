using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    //Normal Attack buff
    public bool forthNormalAttack = false; // add the forth attack to MC
    public bool normalAttackSlash = false; //MC third attack has a slash forward

    //Speacial Attack buff
    public int specialAttack = 1; //1 for spin ,2 for big sword

    public bool specialAdd1 = false; // make special bigger

    public bool specialAdd2 = false; // make special bigger
    public bool specialAdd3 = false; // make special bigger

    public string qskillName = "normal"; // lightning
}
