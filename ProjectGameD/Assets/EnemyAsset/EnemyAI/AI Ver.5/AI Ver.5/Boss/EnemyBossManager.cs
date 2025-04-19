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

        [Header("Boss Defeat Actions")]
        [SerializeField] private GameObject trapdoor;
        [SerializeField] private GameObject pointLight;
        [SerializeField] private GameObject nextStage;

        private bool bossDefeated = false;

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

            if (currentHealth <= 0 && !bossDefeated)
            {
                HandleBossDefeat();
                bossDefeated = true;
            }
        }

        public void ShiftToSecondPhase()
        {
            enemyAnimatorManager.animator.SetBool("isInvulnerable", true);
            enemyAnimatorManager.animator.SetBool("isPhaseShifting", true);
            enemyAnimatorManager.PlayTargetAnimation("PhaseShift", true);
            bossCombatStanceState.hasPhaseShifted = true;

        }

        private void HandleBossDefeat()
        {
            Debug.Log("Boss defeated! Triggering post-battle events.");

            // Open trapdoor (if using animation, trigger here instead)
            if (trapdoor != null)
                trapdoor.SetActive(false); // or SetActive(true) depending on your logic

            // Activate point light
            if (pointLight != null)
                pointLight.SetActive(true);

            // Activate next stage
            if (nextStage != null)
                nextStage.SetActive(true);
        }
    }
}
