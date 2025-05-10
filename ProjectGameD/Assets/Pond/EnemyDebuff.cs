using UnityEngine;
namespace SG
{

    public class EnemyDebuff : MonoBehaviour
    {
        public float rotationSlowMultiplier;

        [SerializeField] EnemyManager enemyManager;
        [SerializeField] EnemyStat enemyStat;

        public float damageMultiplier = 1.5f;
        public Knife stuckKnife;

        void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyStat = GetComponent<EnemyStat>();
        }


        public void ApplyDebuff(Knife knife)
        {
            stuckKnife = knife;
            ApplyDebuffCoroutine();
        }


        private void ApplyDebuffCoroutine()
        {
            if (enemyManager != null) enemyManager.rotationSpeed *= rotationSlowMultiplier;

            if (enemyStat != null)
            {
                float healthPercent = (float)enemyStat.GetCurrentHealth() / enemyStat.GetMaxHealth();
                Debug.Log(healthPercent);

                if (healthPercent <= 0.25f)
                {
                    enemyManager.canBeRiposted = true;
                }
            }
        }

    }
}