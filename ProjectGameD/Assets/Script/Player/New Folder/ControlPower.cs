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

    public GameObject[] SpecialVfxBig; //k button
    public GameObject[] SpecialVfxAdd2; //k button
    public GameObject[] SpecialVfxAdd3; //k button
    public GameObject[] SpecialVfxAdd2and3; //k button

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
    }

    void Update()
    {
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
            case 0:
                playerCombat.skillVFX[0] = SpecialVfx[0];
                break;
            case 1:
                if (characterData.specialAdd2 && characterData.specialAdd3)
                {
                    playerCombat.skillVFX[0] = SpecialVfxAdd2and3[0];
                    break;
                }
                else if (characterData.specialAdd2)
                {
                    playerCombat.skillVFX[0] = SpecialVfxAdd2[0];
                }
                else if (characterData.specialAdd3)
                {
                    playerCombat.skillVFX[0] = SpecialVfxAdd3[0];
                }
                else
                {
                    playerCombat.skillVFX[0] = SpecialVfx[1];
                }
                break;
        }
    }
}
