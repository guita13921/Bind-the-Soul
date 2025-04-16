using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

public class BlockingColliderPlayer : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] AnimatorHander animatorHander;
    [SerializeField] WeaponSlotManager weaponSlotManager;
    public BoxCollider blockingCollider;
    public float blockingColliderDamageAbsorption; // Using Both Player/Enemy

    [Header("Stamina")]
    // public UIEnemyShieldBar uIEnemyShieldBar;
    private float lastAttackTime;
    private Coroutine regenCoroutine;

    private void Awake()
    {
        animatorHander = GetComponentInParent<AnimatorHander>();
        playerManager = GetComponentInParent<PlayerManager>();
        blockingCollider = GetComponent<BoxCollider>();
    }
    public void SetColliderDamageAbsorption(WeaponItem weapon)
    {
        if (weapon != null)
        {
            blockingColliderDamageAbsorption = weapon.physicalDamageAbsorption;
        }
    }

    public void EnableBlockingCollider()
    {
        blockingCollider.enabled = true;
    }

    public void DisableBlockingCollider()
    {
        blockingCollider.enabled = false;
    }
}
