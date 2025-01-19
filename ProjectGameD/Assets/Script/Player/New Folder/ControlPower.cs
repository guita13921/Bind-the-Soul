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
    public GameObject[] SpecialVfxAdd1; //k button
    public GameObject[] SpecialVfxAdd2; //k button
    public GameObject[] SpecialVfxAdd1and2; //k button

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
        // StartCoroutine(InstantiateSkillCoroutine());
    }

    void Update()
    {
        CheckQskill();
        CheckSpecialskill();
    }

    // IEnumerator InstantiateSkillCoroutine()
    // {
    //     while (true)
    //     {
    //         Instantiate(skills[1], parentObject);
    //         float randomWaitTime = Random.Range(0.1f, 1.0f);

    //         yield return new WaitForSeconds(randomWaitTime);
    //     }
    // }

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

    public void SetPlayerSkillVFX(
        CharacterData characterData,
        PlayerCombat playerCombat,
        int skillnum
    )
    {
        if (characterData.specialAdd1 && characterData.specialAdd2)
        {
            playerCombat.skillVFX[0] = SpecialVfxAdd1and2[skillnum];
        }
        else if (characterData.specialAdd1)
        {
            playerCombat.skillVFX[0] = SpecialVfxAdd1[skillnum];
        }
        else if (characterData.specialAdd2)
        {
            playerCombat.skillVFX[0] = SpecialVfxAdd2[skillnum];
        }
        else
        {
            playerCombat.skillVFX[0] = SpecialVfx[skillnum];
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
                SetPlayerSkillVFX(characterData, playerCombat, 1);
                break;
            case 2:
                SetPlayerSkillVFX(characterData, playerCombat, 2);
                break;
            case 3:
                SetPlayerSkillVFX(characterData, playerCombat, 3);
                break;
        }
    }
}
