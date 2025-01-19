using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{
    [SerializeField]
    private Image KCooldown;

    [SerializeField]
    private Image QCooldown;

    [SerializeField]
    private PlayerCombat playerCombat;
    public GameObject Kobj;
    public GameObject Qobj;

    void Update()
    {
        KUpdatecooldown();
        QUpdatecooldown();
    }

    private void KUpdatecooldown()
    {
        Debug.Log(
            (playerCombat.specialAttackCooldown - playerCombat.timeSinceLastSpecialAttack)
                / playerCombat.specialAttackCooldown
        );
        KCooldown.fillAmount = Mathf.Lerp(
            KCooldown.fillAmount,
            (playerCombat.specialAttackCooldown - playerCombat.timeSinceLastSpecialAttack)
                / playerCombat.specialAttackCooldown,
            Time.deltaTime * 10
        );
        Debug.Log(
            Mathf.Lerp(
                KCooldown.fillAmount,
                (playerCombat.specialAttackCooldown - playerCombat.timeSinceLastSpecialAttack)
                    / playerCombat.specialAttackCooldown,
                Time.deltaTime * 10
            )
        );
        if (playerCombat.isSpecialAttackReady)
        {
            Kobj.SetActive(false);
        }
    }

    private void QUpdatecooldown()
    {
        QCooldown.fillAmount = Mathf.Lerp(
            QCooldown.fillAmount,
            (playerCombat.castCooldown - playerCombat.timeSinceLastCast)
                / playerCombat.castCooldown,
            Time.deltaTime * 10
        );
        if (playerCombat.isCastReady)
        {
            Qobj.SetActive(false);
        }
    }
}
