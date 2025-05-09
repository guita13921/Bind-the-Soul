using UnityEngine;
namespace SG
{

    public class EnemyDebuff : MonoBehaviour
    {
        public float slowMultiplier = 0.5f;
        public float rotationSlowMultiplier = 0.5f;
        public float damageMultiplier = 1.5f;
        public float debuffDuration = 5f;

        public Knife stuckKnife;

        //private EnemyMovement movement;
        // private EnemyCombat combat;

        void Awake()
        {
            //movement = GetComponent<EnemyMovement>();
            //combat = GetComponent<EnemyCombat>();
        }


        public void ApplyDebuff(Knife knife)
        {
            stuckKnife = knife;
            //StartCoroutine(ApplyDebuffCoroutine());
        }

        /*
                private System.Collections.IEnumerator ApplyDebuffCoroutine()
                {
                    if (movement != null) movement.ModifySpeed(slowMultiplier);
                    if (movement != null) movement.ModifyRotationSpeed(rotationSlowMultiplier);
                    if (combat != null) combat.ModifyDamageTaken(damageMultiplier);

                    yield return new WaitForSeconds(debuffDuration);

                    if (movement != null) movement.ModifySpeed(1f); // Reset to normal
                    if (movement != null) movement.ModifyRotationSpeed(1f);
                    if (combat != null) combat.ModifyDamageTaken(1f);
                }
        */
    }
}