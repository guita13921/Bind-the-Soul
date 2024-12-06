using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    //Normal Attack buff
    public bool forthNormalAttack = false;
    public bool normalAttackSlash = false;
}
