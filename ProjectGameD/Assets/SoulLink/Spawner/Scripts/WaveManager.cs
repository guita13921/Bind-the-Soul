using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Wave Manager")]
    public class WaveManager : MonoBehaviour, ISoulLinkGuid
    {
        public string LevelName
        { 
            get { return _levelName; } 
            set { _levelName = value; }
        }

        [SerializeField]
        private string _levelName;

        public List<WaveSpawner> Waves
        {
            get { return _waves; }
        }
        [SerializeField]
        private List<WaveSpawner> _waves = new List<WaveSpawner>();

        [Tooltip("Toggle this if you want a wave repeat to count as a new wave number.")]
        [SerializeField]
        private bool _accumulateWaveRepeats = true;

        public int WaveNumber
        {
            get { return CalcWaveNumber(); }
        }

        public int CurrentWaveIndex
        {
            get { return _currentWaveIndex; }
            set { _currentWaveIndex = value; }
        }

        // The wave we are currently on
        private int _currentWaveIndex = 0;

        public LevelManager LevelManager
        { get { return _levelManager != null ? _levelManager : GetComponentInParent<LevelManager>(); } }

        // The level manager that this wave manager belongs to
        private LevelManager _levelManager;

        public string _objectGuid;

        private void Awake()
        {
            // Deactivate all wave objects before starting
            foreach (var wave in _waves)
            {
                wave.gameObject.SetActive(false);
            } // foreach
        } // Awake()

        void Start()
        {
            ActivateCurrentWave();
        } // Start()

        public void SetLevelManager(LevelManager levelManager)
        {
            _levelManager = levelManager;
        } // SetLevelManager()

        /// <summary>
        /// Generate a unique id for this object
        /// </summary>
        public void GenerateGuid()
        {
            if (string.IsNullOrEmpty(_objectGuid))
            {
                _objectGuid = Guid.NewGuid().ToString();
            } // if
        } // GenerateGuid()

        /// <summary>
        /// Get the object's Guid
        /// </summary>
        /// <returns></returns>
        public string GetGuid()
        {
            return _objectGuid;
        } //GetGuid()

        /// <summary>
        /// Set the object's Guid directly
        /// </summary>
        /// <param name="value"></param>
        public void SetGuid(string value)
        {
            _objectGuid = value;
        } // SetGuid()

        /// <summary>
        /// Activate all waves that fall within the current time of day
        /// </summary>
        void ActivateWaves()
        {
            foreach (var wave in _waves)
            {
                if (SoulLinkSpawner.Instance.IsInCurrentTimeOfDay(wave.TimeOfDay))
                {
                    wave.gameObject.SetActive(true);
                } // if
            } // foreach
        } // ActivateWaves()

        /// <summary>
        /// Activates the current wave
        /// </summary>
        void ActivateCurrentWave()
        {
            _waves[_currentWaveIndex].gameObject.SetActive(true);
        } // ActivateCurrentWave()

        /// <summary>
        /// Calculate the wave number considering wave repeats as their own wave if _accumulateWaveRepeats is true
        /// </summary>
        /// <returns></returns>
        public int CalcWaveNumber()
        {
            int total = 1 + _currentWaveIndex;

            for (int i = 0; i < _waves.Count; ++i)
            {
                if (i > _currentWaveIndex) break;

                if (_accumulateWaveRepeats && _waves[i].RepeatWave)
                {
                    total += _waves[i].CurrentRepeat;
                }
            } // foreach

            return total;
        } // CalcWaveNumber()

        /// <summary>
        /// WaveSpawner will call this when its wave is completed
        /// </summary>
        public void WaveCompleted()
        {
            ++_currentWaveIndex;
            if (_currentWaveIndex < _waves.Count)
            {
                // Activate the next wave
                ActivateCurrentWave();
            } // if
        } // WaveCompleted()

        /// <summary>
        /// Get the total number of waves for all wave spawners; consider their repeat count values in the total
        /// </summary>
        /// <returns></returns>
        public int GetWavesCount()
        {
            int totalWaves = 0;

            foreach (var wave in _waves)
            {
                totalWaves += 1 + wave.NumberOfRepeats;
            } // foreach

            return totalWaves;
        } // GetWavesCount()

        /// <summary>
        /// Reset this component to its starting conditions
        /// </summary>
        public void ResetToDefault()
        {
            // Reset all wave spawners
            foreach (var waveSpawner in _waves)
            {
                waveSpawner.ResetWave();
            } // foreach

            // Reset this component
            _currentWaveIndex = 0;
        } // ResetToDefault()
    } // WaveManager
} // namespace Magique.SoulLink
