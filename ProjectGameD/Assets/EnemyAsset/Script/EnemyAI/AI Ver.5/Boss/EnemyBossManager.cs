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
        [SerializeField] EnemyAnimatorManager enemyAnimatorManager;
        BossCombatStanceState bossCombatStanceState;

        [Header("Second Phase FX")]
        public GameObject particleFX;

        private void Awake()
        {
            uIBossHealthBar = FindObjectOfType<UIBossHealthBar>();
            enemyStat = GetComponent<EnemyStat>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            bossCombatStanceState = GetComponentInChildren<BossCombatStanceState>();
        }

        void Start()
        {
            uIBossHealthBar.SetBossName(bossName);
            uIBossHealthBar.SetBossMaxHealth(enemyStat.maxHealth);
        }

        public void UpdateBossHealthBar(int currentHealth, int maxHealth)
        {
            uIBossHealthBar.SetBossCurrentHealth(currentHealth);

            if (currentHealth <= maxHealth / 2 && !bossCombatStanceState.hasPhaseShifted)
            {
                bossCombatStanceState.hasPhaseShifted = true;
                ShiftToSecondPhase();
            }
        }

        public void ShiftToSecondPhase()
        {
            enemyAnimatorManager.animator.SetBool("isInvulnerable", true);
            enemyAnimatorManager.animator.SetBool("isPhaseShifting", true);
            enemyAnimatorManager.PlayTargetAnimation("PhaseShift", true);
            bossCombatStanceState.hasPhaseShifted = true;
        }
    }
}