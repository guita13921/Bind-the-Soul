using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownBar2 : MonoBehaviour
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

    [SerializeField]
    Image Kimage;

    [SerializeField]
    Image Qimage;

    private void KUpdatecooldown()
    {
        KCooldown.fillAmount = Mathf.Lerp(
            KCooldown.fillAmount,
            (playerCombat.specialAttackCooldown - playerCombat.timeSinceLastSpecialAttack2)
                / playerCombat.specialAttackCooldown,
            Time.deltaTime * 100
        );

        if (playerCombat.isSpecialAttackReady2)
        {
            Kobj.SetActive(false);
            Kimage.color = new Color(255f, 217f, 0f);
        }
    }

    private void QUpdatecooldown()
    {
        QCooldown.fillAmount = Mathf.Lerp(
            QCooldown.fillAmount,
            (playerCombat.castCooldown - playerCombat.timeSinceLastCast2)
                / playerCombat.castCooldown,
            Time.deltaTime * 100
        );
        if (playerCombat.isCastReady2)
        {
            Qobj.SetActive(false);
            Qimage.color = new Color(255f, 217f, 0f);
        }
    }
}
