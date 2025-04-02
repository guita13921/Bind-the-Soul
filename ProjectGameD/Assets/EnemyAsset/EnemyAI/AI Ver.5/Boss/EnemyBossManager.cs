using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class EnemyBossManager : MonoBehaviour
    {
        public string bossName;
        UIBossHealthBar uIBossHealthBar;
        EnemyStat enemyStat;

        //HANDLE SWICH PHASE
        //HANDLE SWICHING ATTACK PATTERN
        private void Awake()
        {
            uIBossHealthBar = FindObjectOfType<UIBossHealthBar>();
            enemyStat = GetComponent<EnemyStat>();
        }

        void Start()
        {
            uIBossHealthBar.SetBossName(bossName);
            uIBossHealthBar.SetBossMaxHealth(enemyStat.maxHealth);
        }
    }
}