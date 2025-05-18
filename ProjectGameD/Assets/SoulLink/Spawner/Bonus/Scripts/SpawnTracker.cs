using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnTracker : MonoBehaviour, ISpawnEvents
    {
        [SerializeField]
        private int _pointsValue;

        private bool _hasDied = false;

        public void OnSpawned()
        {
            _hasDied = false;
            SpawnTrackerManager.Instance.SpawnCount++;
        } // OnSpawned()

        public void OnDeath()
        {
            _hasDied = true;
            SpawnTrackerManager.Instance.SpawnCount--;
            SpawnTrackerManager.Instance.Score += _pointsValue;
        } // OnDeath()

        public void OnDespawned()
        {
            if (!_hasDied)
            {
                SpawnTrackerManager.Instance.SpawnCount--;
            } // if
        } // OnDespawned()
    } // class SpawnTracker
} // namespace Magique.SoulLink