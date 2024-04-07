using UnityEngine;

namespace EnemyAI.GOAP.Config
{
    [CreateAssetMenu(menuName = "AI/Wander Config", fileName = "Wander Config", order = 1)]
    public class WanderConfigSO : ScriptableObject
    {
        public Vector2 WaitRangeBetweenWanders = new(1, 5);
        public float WanderRadius = 5f;
    }
}