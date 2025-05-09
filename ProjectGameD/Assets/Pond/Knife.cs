using UnityEngine;

namespace SG
{

    public class Knife : MonoBehaviour
    {
        public Transform target;
        private EnemyDebuff stuckEnemy; // to verify later
        public PlayerAttack ownerAttack;

        public float knifeBaseDamage;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Enemy") || collider.transform == target)
            {
                EnemyDebuff debuff = collider.GetComponent<EnemyDebuff>();

                if (debuff != null)
                {
                    stuckEnemy = debuff; // Save it for later verification
                    debuff.ApplyDebuff(this); // Pass reference to this knife
                }

                Transform daggerPos = collider.transform.Find("Skeleton_model/Dagger Position");

                if (daggerPos != null)
                {
                    StickToEnemy(daggerPos);
                    if (!collider.CompareTag("Enemy"))
                        return;

                    EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
                    EnemyManager enemyManager = collider.GetComponent<EnemyManager>();
                    BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

                    float damage = knifeBaseDamage;

                    /*
                    Modify for power-up 
                    */

                    int currentDamage = Mathf.RoundToInt(damage);

                    if (enemyStat != null)
                    {
                        if (enemyStat.isBoss || enemyManager.isStunning)
                            enemyStat.TakeDamageNoAnimation(currentDamage);
                        else
                            enemyStat.TakeDamage(currentDamage);
                    }

                    //ApplyDebuff(other.transform);
                }
                else
                {
                    StickToEnemy(collider.transform); // Fallback
                    Debug.Log("Fallback");
                }

            }
        }


        void StickToEnemy(Transform enemy)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = enemy;
        }


    }
}