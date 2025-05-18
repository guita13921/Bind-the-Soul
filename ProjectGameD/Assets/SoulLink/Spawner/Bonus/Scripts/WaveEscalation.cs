using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magique.SoulLink
{
    public class WaveEscalation : MonoBehaviour
    {
        [Tooltip("How many days before the next disabled wave item should be enabled?")]
        [SerializeField]
        private int _daysToEnable = 3;

        [Tooltip("How many days before enabled wave items should escalate?")]
        [SerializeField]
        private int _daysToEscalate = 1;

        [Tooltip("The percentage of the current spawns to increase by (10-100).")]
        [Range(10,100)]
        [SerializeField]
        private int _escalatePercentage = 100;

        private WaveSpawner _waveSpawner;
        private int _waveIndex = 1;
        private int _dayCount = 0;

        private void Awake()
        {
            _waveSpawner = GetComponent<WaveSpawner>();
        } // Awake()

        public void OnWaveCompleted()
        {
            ++_dayCount;

            if (_dayCount % _daysToEscalate == 0)
            {
                EscalateEnabledWaves();
            }

            if (_dayCount % _daysToEnable == 0 && _waveIndex < _waveSpawner.WaveItems.Count)
            {
                _waveSpawner.WaveItems[_waveIndex++].disable = false;
            }
        } // OnWaveCompleted()

        public void EscalateEnabledWaves()
        {
            foreach (var waveItem in _waveSpawner.WaveItems)
            {
                if (!waveItem.disable)
                {
                    waveItem.minMaxSpawns.x += (int)((waveItem.minMaxSpawns.x * (_escalatePercentage / 100f)) + 0.5f);
                    waveItem.minMaxSpawns.y += (int)((waveItem.minMaxSpawns.y * (_escalatePercentage / 100f)) + 0.5f);
                } // if
            } // foreach
        } // EscalateEnabledWaves()
    }
} // namespace Magique.SoulLink