using UnityEngine;

namespace EnemyAI.GOAP.Config
{
    [CreateAssetMenu(menuName = "AI/Attack Config", fileName = "Attack Config", order = 2)]
    public class AttackConfigSO : ScriptableObject
    {
        public float SensorRadius = 1;
        public float MeleeAttackRadius = 1f;
        public int MeleeAttackCost = 1;
        public float AttackDelay = 1;
        public LayerMask AttackableLayerMask;
    }
}