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
        public GameObject hitFXPrefab; // Assign this in Inspector
        public Transform hitEffectSpawnPoint; // Optional: empty GameObject to set exact location
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

        public void PlayHitEffect()
        {
            if (hitFXPrefab != null)
            {
                // Use spawn point if assigned, otherwise fallback to enemy's transform
                Vector3 basePos = hitEffectSpawnPoint != null ? hitEffectSpawnPoint.position : transform.position;

                // ✨ Add random offset in a small radius (XZ plane for most use-cases)
                float offsetRadius = 0.3f;
                Vector2 randomOffset2D = Random.insideUnitCircle * offsetRadius;
                Vector3 randomOffset = new Vector3(randomOffset2D.x, 0f, randomOffset2D.y);

                Vector3 spawnPos = basePos + randomOffset;
                Quaternion spawnRot = Quaternion.identity;

                GameObject hitFX = Instantiate(hitFXPrefab, spawnPos, spawnRot);
                Destroy(hitFX, 1f);
            }
            else
            {
                Debug.LogWarning("Hit FX Prefab is not assigned on EnemyEffectManager.");
            }
        }

    }
}