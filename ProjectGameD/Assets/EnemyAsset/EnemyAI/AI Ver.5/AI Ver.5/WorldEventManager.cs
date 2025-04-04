using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class WorldEventManager : MonoBehaviour
    {
        UIBossHealthBar uIBossHealthBar;
        EnemyBossManager boss;

        public bool bossFightISActive;// currently fight
        public bool bossHasBeenAwakened;// Woek in cutscene
        public bool bossHasBeenDefeated;// Boss Dead

        private void Awake()
        {
            uIBossHealthBar = FindObjectOfType<UIBossHealthBar>();
        }

        public void ActivateBossFight()
        {
            bossFightISActive = true;
            bossHasBeenAwakened = true;
            uIBossHealthBar.SetUIHealthBarToActive();
        }

        public void BossHasBeenDefeated()
        {
            bossFightISActive = false;
            bossHasBeenDefeated = true;
        }
    }
}