using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;


namespace SG
{
    public class EnemyEffectManager : CharacterEffectManager
    {
        EnemyStat enemyStat;
        EnemyWeaponSlotManager enemyWeaponSlotManager;
        EnemyAnimatorManager enemyAnimatorManager;

        public GameObject currentParticalFX;
        public GameObject instantiatedFXModel;
        public int amountToBeHealed;

        private void Awake()
        {
            enemyStat = GetComponent<EnemyStat>();
            enemyWeaponSlotManager = GetComponent<EnemyWeaponSlotManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        void Update()
        {
            CheckisAttacking();
        }

        void CheckisAttacking()
        {
            if (enemyAnimatorManager.animator.GetBool("isAttacking") == true)
            {
                PlayWeaponFX(false);
            }
            else
            {
                StopWeaponFX(false);
            }
        }

    }
}