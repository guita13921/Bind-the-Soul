using UnityEngine;

namespace SG
{

    public class Knife : MonoBehaviour
    {
        public Transform target;
        private EnemyDebuff stuckEnemy; // to verify later
        public PlayerAttack ownerAttack;

        public float knifeBaseDamage;
        private bool hasHit = false; // ✅ Add this

        private void OnTriggerEnter(Collider collider)
        {
            if (hasHit) return; // ✅ Prevent double hit
            if (!(collider.CompareTag("Enemy") || collider.transform == target)) return;

            hasHit = true; // ✅ Mark as hit to avoid further trigger

            Transform daggerPos = collider.transform.Find("Combat Transform/Dagger Position");

            if (daggerPos != null)
            {
                StickToEnemy(daggerPos);
            }
            else
            {
                StickToEnemy(collider.transform);
                Debug.Log("Fallback");
            }

            if (!collider.CompareTag("Enemy"))
                return;

            EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
            EnemyManager enemyManager = collider.GetComponent<EnemyManager>();
            BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

            float damage = knifeBaseDamage;

            int currentDamage = Mathf.RoundToInt(damage);

            if (enemyStat != null)
            {
                if (enemyStat.isBoss || (enemyManager != null && enemyManager.isStunning))
                    enemyStat.TakeDamageNoAnimation(currentDamage);
                else
                    enemyStat.TakeDamage(currentDamage);
            }

            EnemyDebuff debuff = collider.GetComponent<EnemyDebuff>();
            if (debuff != null)
            {
                stuckEnemy = debuff;
                debuff.ApplyDebuff(this);
            }
        }


        void StickToEnemy(Transform enemy)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = enemy;
        }


    }
}