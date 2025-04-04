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

    }

}