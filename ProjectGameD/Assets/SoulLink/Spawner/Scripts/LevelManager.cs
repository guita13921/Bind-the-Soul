using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Level Manager")]
    public class LevelManager : MonoBehaviour, ISoulLinkGuid
    {
        [SerializeField]
        private bool _autoStart = true;

        [Tooltip("Specify a level index to start from. the default is 0, which is the first level in the manager.")]
        [SerializeField]
        private int _startLevelIndex = 0;
        
        public List<WaveManager> Levels
        { get { return _levels; } }

        [SerializeField]
        private List<WaveManager> _levels = new List<WaveManager>();

        [Tooltip("Speficy a sound to play when a level is started.")]
        [SerializeField]
        private AudioClipInfo _onLevelStartedSound;

        [Tooltip("Speficy a sound to play when a level is ended.")]
        [SerializeField]
        private AudioClipInfo _onLevelCompleteSound;

#region Sound
        [SerializeField]
        private AudioMixerGroup _outputAudioMixerGroup = null;
#endregion

        public int CurrentLevelIndex
        {
            get { return _currentLevelIndex; }
            set { _currentLevelIndex = value; }
        }

        // The level we are currently on
        private int _currentLevelIndex = 0;
        private int _currentStartedIndex = -1;

        [SerializeField]
        private UnityEvent _onLevelStartedEventHandler;

        [SerializeField]
        private UnityEvent _onLevelCompleteEventHandler;

        private string _objectGuid;

        private void Awake()
        {
            // Deactivate all wave objects before starting
            foreach (var level in _levels)
            {
                level.SetLevelManager(this);
                level.gameObject.SetActive(false);
            } // foreach
        } // Awake()

        void Start()
        {
            if (_autoStart)
            {
                _currentLevelIndex = _startLevelIndex;
                StartLevel(_currentLevelIndex);
            } // if
        } // Start()

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
        /// Manually start a level
        /// </summary>
        public void StartLevel(int levelIndex)
        {
            if (levelIndex >= _levels.Count) return;

            if (_onLevelStartedEventHandler != null)
            {
                _onLevelStartedEventHandler.Invoke();
            } // if

            _currentLevelIndex = _currentStartedIndex = levelIndex;
            _levels[_currentLevelIndex].gameObject.SetActive(true);
        } // StartLevel()

        /// <summary>
        /// Complete the current level
        /// </summary>
        public void CompleteLevel()
        {
            if (_onLevelCompleteEventHandler != null && (_currentLevelIndex == _currentStartedIndex))
            {
                _onLevelCompleteEventHandler.Invoke();
            } // if

            if (_onLevelCompleteSound != null)
            {
                PlaySound(_onLevelCompleteSound);
            } // if
        } // CompleteLevel()

        /// <summary>
        /// End the current level and start the next level
        /// </summary>
        public void NextLevel()
        {
            if (_onLevelStartedSound != null)
            {
                PlaySound(_onLevelStartedSound);
            } // if

            StartLevel(_currentLevelIndex + 1);
        } // NextLevel()

        /// <summary>
        /// Play a sound using this manager's sound settings and the referenced clip's volume
        /// </summary>
        /// <param name="audioClipInfo"></param>
        public void PlaySound(AudioClipInfo audioClipInfo)
        {
            if (audioClipInfo == null) return;
            SoulLinkGlobal.Instance.PlaySound(new SoundData()
            {
                audioClip = audioClipInfo.clip,
                soundIs3D = audioClipInfo.is3D,
                soundMinRange = audioClipInfo.minRange,
                soundMaxRange = audioClipInfo.maxRange,
                soundVolume = audioClipInfo.volume,
                outputAudioMixerGroup = _outputAudioMixerGroup
            }, Vector3.zero);
        } // PlaySound()

        /// <summary>
        /// Reset LevelManager to starting conditions
        /// </summary>
        /// <param name="autoStart"></param>
        public void ResetToDefault(bool autoStart = false)
        {
            // Reset all wave manager components
            foreach (var level in _levels)
            {
                level.ResetToDefault();
            } // forach

            // Reset this component and auto start if specified
            _currentLevelIndex = _startLevelIndex;
            if (autoStart)
            {
                StartLevel(_currentLevelIndex);
            } // if
        } // ResetToDefault()
    } // LevelManager
} // namespace Magique.SoulLink
