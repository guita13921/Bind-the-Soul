using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class EnemyEquipmentManager : MonoBehaviour
    {
        BlockingCollider blockingCollider;
        EnemyWeaponSlotManager enemyWeaponSlotManager;


        void Awake()
        {
            enemyWeaponSlotManager = GetComponentInParent<EnemyWeaponSlotManager>();
            blockingCollider = GetComponentInChildren<BlockingCollider>();
        }

        public void OpenBlockCollider()
        {
            blockingCollider.SetColliderDamageAbsorption(enemyWeaponSlotManager.leftHandWeapon);
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider()
        {
            blockingCollider.DisableBlockingCollider();
        }
    }

}