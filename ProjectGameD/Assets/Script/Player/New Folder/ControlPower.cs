using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ControlPower : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public CharacterData characterData;
    public Transform parentObject;
    public PlayerControl playerControl;
    public Health health;
    bool forthAttack = false;
    bool normalAttackSlash = false;

    public GameObject[] skills;
    public GameObject[] dashVFX;
    public GameObject[] SpecialVfx; //k button

    public GameObject[] qSkillVFX;

    public List<AttackSO> attackSOs;

    string qSkillsname;
    int speicalSkills;
    int specialVfxlv = 0;
    private int qSkillTpye;

    void Start()
    {
        if (forthAttack)
        {
            playerCombat.combo.Add(attackSOs[0]);
        }
        // StartCoroutine(InstantiateSkillCoroutine());
        CheckSpeed();
        CheckQKCooldown();
        CheckWaitDashtime();
    }

    void Update()
    {
        CheckQskill();
        // CheckSpecialskill();
        CheckSpeical();
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

    public void CheckSpeed()
    {
        playerControl._speed = playerControl._speed + (1 * characterData.moveFaster);
    }

    public void StartVFX()
    {
        if (normalAttackSlash && playerCombat.comboCounter == 2)
        {
            Instantiate(skills[0], parentObject);
        }
    }

    private void CheckSpeical()
    {
        specialVfxlv = characterData.specialLV;
        playerCombat.speicalVFX[0] = SpecialVfx[specialVfxlv];
    }

    public void CheckQskill()
    {
        qSkillTpye = characterData.QSkillType;
        playerCombat.qSkill[0] = qSkillVFX[qSkillTpye];
    }

    public void CheckWaitDashtime()
    {
        playerControl.dashWaitTime =
            playerControl.dashWaitTime - (0.15f * characterData.ReduceDashCooldown);
    }

    public void CheckQKCooldown()
    {
        playerCombat.castCooldown = playerCombat.castCooldown - characterData.QKReduceCooldown;
        playerCombat.specialAttackCooldown =
            playerCombat.specialAttackCooldown - characterData.QKReduceCooldown;
    }

    
}