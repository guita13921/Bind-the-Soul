using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class DemonBossManager : MonoBehaviour
    {
        public string bossName;
        [SerializeField] UIBossHealthBar uIBossHealthBar;
        EnemyStat enemyStat;

        private void Awake()
        {
            //uIBossHealthBar = FindObjectOfType<UIBossHealthBar>();
            enemyStat = GetComponent<EnemyStat>();
        }

        void Start()
        {
            uIBossHealthBar.SetBossName(bossName);
            uIBossHealthBar.SetBossMaxHealth(enemyStat.maxHealth);
        }

        public void UpdateBossHealthBar(int currentHealth, int maxHealth)
        {
            uIBossHealthBar.SetBossCurrentHealth(currentHealth);
        }

    }
}