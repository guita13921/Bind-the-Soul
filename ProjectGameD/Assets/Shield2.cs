using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield2 : MonoBehaviour
{
    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();

        if (playerCombat != null)
        {
            StartCoroutine(ActivateShieldAfterDelay());
        }
        else
        {
            Debug.LogError("PlayerCombat script not found in parent object.");
        }
    }

    private IEnumerator ActivateShieldAfterDelay()
    {
        yield return new WaitForSeconds(2.45f);
        playerCombat.isShield2 = false;
    }
}
