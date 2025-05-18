#if EMERALD_AI_PRESENT
using EmeraldAI;
using UnityEngine;

// (c)2021 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class RandomNavOffset : MonoBehaviour, ISpawnEvents
    {
        [SerializeField]
        private float _minOffset = 0f;

        [SerializeField]
        private float _maxOffset = 8f;

        public void OnSpawned()
        {
            GetComponent<EmeraldAISystem>().AgentBaseOffset = Random.Range(_minOffset, _maxOffset);
        } // OnSpawned()

        public void OnDespawned()
        {
            // do nothing
        } // OnDespawned()
    } // class RandomNavOffset
} // namespace Magique.SoulLink
#endif