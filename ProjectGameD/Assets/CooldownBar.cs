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

    [SerializeField]
    private GameObject cooldown2;
    public GameObject Kobj;
    public GameObject Qobj;
    public CharacterData characterData;

    void Update()
    {
        KUpdatecooldown();
        QUpdatecooldown();
        if (characterData.Q2_QKStackable)
        {
            cooldown2.SetActive(true);
        }
    }

    [SerializeField]
    Image Kimage;

    [SerializeField]
    Image Qimage;

    private void KUpdatecooldown()
    {
        KCooldown.fillAmount = Mathf.Lerp(
            KCooldown.fillAmount,
            (playerCombat.specialAttackCooldown - playerCombat.timeSinceLastSpecialAttack)
                / playerCombat.specialAttackCooldown,
            Time.deltaTime * 100
        );

        if (playerCombat.isSpecialAttackReady)
        {
            Kobj.SetActive(false);
            Kimage.color = new Color(255f, 217f, 0f);
        }
    }

    private void QUpdatecooldown()
    {
        QCooldown.fillAmount = Mathf.Lerp(
            QCooldown.fillAmount,
            (playerCombat.castCooldown - playerCombat.timeSinceLastCast)
                / playerCombat.castCooldown,
            Time.deltaTime * 100
        );
        if (playerCombat.isCastReady)
        {
            Qobj.SetActive(false);
            Qimage.color = new Color(255f, 217f, 0f);
        }
    }
}
