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

        public GameObject currentParticalFX;
        public GameObject instantiatedFXModel;
        public int amountToBeHealed;

        private void Awake()
        {
            enemyStat = GetComponent<EnemyStat>();
            enemyWeaponSlotManager = GetComponent<EnemyWeaponSlotManager>();
        }


    }
}