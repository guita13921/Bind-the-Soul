using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPower : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public CharacterData characterData;
    public Transform parentObject;
    bool forthAttack = false;
    bool normalAttackSlash = false;

    public GameObject[] skills;
    public GameObject[] dashVFX;

    public List<AttackSO> attackSOs;

    void Start()
    {
        forthAttack = characterData.forthNormalAttack;
        normalAttackSlash = characterData.normalAttackSlash;
        if (forthAttack)
        {
            playerCombat.combo.Add(attackSOs[0]);
        }
    }

    public void DashVFX()
    {
        Instantiate(dashVFX[0], parentObject);
    }

    public void StartVFX()
    {
        if (normalAttackSlash && playerCombat.comboCounter == 2)
        {
            Instantiate(skills[0], parentObject);
        }
    }
}
