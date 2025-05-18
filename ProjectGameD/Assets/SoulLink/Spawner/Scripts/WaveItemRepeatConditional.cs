using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Wave Item Repeat Conditional")]
    public class WaveItemRepeatConditional : MonoBehaviour
    {
        [SerializeField]
        private string _waveItemName;

        [Tooltip("How many repeats are required to perform the conditional spawning.")]
        [SerializeField]
        private int _repeatInterval = 3;

//        [SerializeField]
//        private SpawnType _spawnType;

        [Tooltip("Drag the prefab to spawn here.")]
        [SerializeField]
        private Transform _prefab;

        //        [Tooltip("Drag the herd to spawn here.")]
        //        [SerializeField]
        //        private SpawnerHerd _herd;

        [Tooltip("Specify the quantity of conditional items to spawn.")]
        [SerializeField]
        private int _quantity = 1;

        [Tooltip("Specify an origin transform where you want to spawn the conditional.")]
        [SerializeField]
        private Transform _spawnOrigin;

        [Tooltip("Specify a radius from the origin point to randomly spawn the conditional.")]
        [SerializeField]
        private float _spawnRadius = 3f;

        private int _repeatCount = 0;
        private bool _performConditionalSpawning = false;

        private void Awake()
        {
            var poolManager = FindObjectOfType<PoolManager>();
            if (poolManager != null)
            {
                poolManager.AddPoolItem(_prefab, SoulLinkSpawner.Instance.DefaultPopulationCap, SoulLinkSpawner.Instance.Database);
            } // if
        } // Awake()

        /// <summary>
        /// Handler for when a wave item has completed
        /// </summary>
        /// <param name="waveItem"></param>
        void OnRepeatCompleteEventHandler(WaveItemData waveItem)
        {
            if (_waveItemName.Equals(waveItem.itemName)) ++_repeatCount;

            _performConditionalSpawning = (_repeatCount % _repeatInterval == 0);
        } // OnRepeatCompleteEventHandler()

        /// <summary>
        /// Handler for when a wave item has completed spawning
        /// </summary>
        /// <param name="waveItem"></param>
        void OnSpawningCompleteEventHandler(WaveItemData waveItem)
        {
            if (!_performConditionalSpawning) return;

            // TODO: Future expansion to include more than just a single prefab
            /*                switch (_spawnType)
                            {
                                case SpawnType.Prefab:
                                    break;
                                case SpawnType.Herd:
                                    break;
                                case SpawnType.Variants:
                                    break;
                            } // switch
            */

            for (int i = 0; i < _quantity; ++i)
            {
                var spawnPos = Utility.GetRandomPos(0f, _spawnRadius, _spawnOrigin.position);
                SoulLinkSpawner.Instance.Spawn(new SpawnData()
                { randomChance = 999, spawnPos = spawnPos, spawnPrefab = _prefab, waveItemGuid =  waveItem.guid, waveSpawnerGuid = GetComponent<ISoulLinkGuid>().GetGuid() });
            } // for i

            waveItem.spawnTracker.quota += _quantity;

            _performConditionalSpawning = false;
        } // OnSpawningCompleteEventHander()
    } // class WaveItemRepeatConditional
} // namespace Magique.SoulLink