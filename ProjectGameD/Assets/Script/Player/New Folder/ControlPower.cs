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
    public GameObject[] SpecialVfx; //k button

    public GameObject[] qSkill;

    public List<AttackSO> attackSOs;

    string qSkillsname;

    void Start()
    {
        forthAttack = characterData.forthNormalAttack;
        normalAttackSlash = characterData.normalAttackSlash;
        if (forthAttack)
        {
            playerCombat.combo.Add(attackSOs[0]);
        }
        CheckSpcial();
        CheckQskill();
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

    public void CheckSpcial()
    {
        if (characterData.specialAdd1)
        {
            playerCombat.skillVFX[0] = SpecialVfx[0];
        }
    }

    public void CheckQskill()
    {
        qSkillsname = characterData.qskillName;
        switch (qSkillsname)
        {
            case "normal":
                break;

            case "lightning":
                playerCombat.qSkill[0] = qSkill[0];
                break;
        }
    }
}
