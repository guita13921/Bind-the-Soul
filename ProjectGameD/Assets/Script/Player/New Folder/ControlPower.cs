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
    int speicalSkills;

    void Start()
    {
        forthAttack = characterData.forthNormalAttack;
        normalAttackSlash = characterData.normalAttackSlash;
        if (forthAttack)
        {
            playerCombat.combo.Add(attackSOs[0]);
        }
        CheckQskill();
        CheckSpecialskill();
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

    public void CheckSpecialskill()
    {
        speicalSkills = characterData.specialAttack;
        switch (speicalSkills)
        {
            case 1:
                break;

            case 2:
                playerCombat.skillVFX[0] = SpecialVfx[1];
                break;
        }
    }
}
