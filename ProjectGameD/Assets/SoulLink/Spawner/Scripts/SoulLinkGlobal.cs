using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if SOULLINK_USE_MASTERAUDIO
using DarkTonic.MasterAudio;
#endif

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkGlobal : MonoBehaviour
    {
        static public SoulLinkGlobal Instance { get; private set; }

        public AudioMixerGroup OutputAudioMixerGroup
        {
            get { return _outputAudioMixerGroup; }
            set { _outputAudioMixerGroup = value; }
        }
        [Tooltip("The output mixer group for PlaySound calls.")]
        [SerializeField]
        private AudioMixerGroup _outputAudioMixerGroup;

        public bool DebugMode
        {
            get { return _debugMode; }
        }

        [SerializeField]
        private bool _debugMode = false;

        public bool WriteToFile
        {
            get { return _writeToFile; }
        }

        [SerializeField]
        private bool _writeToFile = false;

        public string LogFileName
        { get { return _logFileName; } }

        [SerializeField]
        private string _logFileName = "SoulLink.log";

        public PoolManager PoolManager
        {
            get { return _poolManager; }
            set { _poolManager = value; }
        }

        private PoolManager _poolManager;

        static StreamWriter logFile;

        private List<AudioSource> _audioSources = new List<AudioSource>();

        private int _maxAudioSources = 32;
        private int _audioSourceIndex = 0;

        public bool versionsFoldoutState = false;

        static private BaseGameTime _gameTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            _poolManager = FindObjectOfType<PoolManager>();

            if (_writeToFile)
            {
                logFile = File.CreateText(_logFileName);
            } // if

            _maxAudioSources = AudioSettings.GetConfiguration().numRealVoices;

            // Create audio sources for general use
            for (int i = 0; i < _maxAudioSources; ++i)
            {
                var obj = new GameObject();
                obj.name = "AudioSource";
                obj.transform.SetParent(transform);
                obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                AudioSource audioSource = obj.AddComponent(typeof(AudioSource)) as AudioSource;
                audioSource.playOnAwake = false;
                audioSource.loop = false;

                _audioSources.Add(audioSource);
            } // for i

            var gametime = Resources.FindObjectsOfTypeAll(typeof(BaseGameTime)) as BaseGameTime[];
            if (gametime.Length != 0)
            {
                _gameTime = gametime[0];
            } // if
        } // Awake()

        public static void DebugLog(GameObject obj, string debugStr, bool showTimestamp = true)
        {
            if (!Instance.DebugMode) return;

            var logStr = "";
            var objName = (obj != null ? (" (" + obj.name + ")") : "");

            string gameTimeStamp = GetGameTimeStamp();

            if (Instance._writeToFile && logFile != null)
            {
                if (showTimestamp)
                {
                    logStr = System.DateTime.Now + ": " + debugStr + objName + " " + gameTimeStamp;
                    logFile.WriteLine(logStr);
                    logFile.Flush();
                }
                else
                {
                    logStr = debugStr + objName + " " + gameTimeStamp;
                    logFile.WriteLine(logStr);
                    logFile.Flush();
                }
            } // if

            // Also log to debug console
            Debug.Log(debugStr + objName + " " + gameTimeStamp);
        } // DebugLog()

        public static string GetGameTimeStamp()
        {
            if (_gameTime != null)
            {
                return string.Format("({0}:{1:00})", _gameTime.GetHour(), _gameTime.GetMinute());
            } // if

            return "";
        } // GetGameTimeStamp()

        private void OnDestroy()
        {
            if (_writeToFile)
            {
                logFile.Close();
            } // if
        } // OnDestroy()

        /// <summary>
        /// Play a 2D or 3D sound without requiring your own audio source
        /// </summary>
        /// <param name="soundData"></param>
        /// <param name="playPosition"></param>
        public void PlaySound(SoundData soundData, Vector3 playPosition)
        {
            if (soundData == null || soundData.audioClip == null) return;

            if (soundData.soundIs3D && Camera.main != null && Vector3.Distance(Camera.main.transform.position, playPosition) > soundData.soundMaxRange) return;

#if SOULLINK_USE_MASTERAUDIO
            if (soundData.soundIs3D)
            {
                MasterAudio.PlaySound(soundData.audioClipName, soundData.soundVolume);
            }
            else
            {
                MasterAudio.PlaySound3DAtVector3(soundData.audioClipName, playPosition, soundData.soundVolume);
            }
#else
            // Use the next audio source from the pool and use it here
            _audioSources[_audioSourceIndex].outputAudioMixerGroup = soundData.outputAudioMixerGroup ? soundData.outputAudioMixerGroup : null;
            _audioSources[_audioSourceIndex].transform.position = playPosition;
            _audioSources[_audioSourceIndex].clip = soundData.audioClip;
            _audioSources[_audioSourceIndex].volume = soundData.soundVolume;
            _audioSources[_audioSourceIndex].spatialBlend = soundData.soundIs3D ? 1f : 0f;
            _audioSources[_audioSourceIndex].minDistance = soundData.soundMinRange;
            _audioSources[_audioSourceIndex].maxDistance = soundData.soundMaxRange;
            _audioSources[_audioSourceIndex].Play();

            _audioSourceIndex++;
            if (_audioSourceIndex >= _maxAudioSources)
            {
                _audioSourceIndex = 0;
            } // if
#endif
        } // PlaySound()

        /// <summary>
        /// Play a 2D or 3D sound without requiring your own audio source
        /// </summary>
        /// <param name="soundData"></param>
        /// <param name="playPosition"></param>
        public void PlaySound(AudioClipInfo clipInfo, Vector3 playPosition)
        {
            if (clipInfo == null) return;

            if (clipInfo.is3D && Camera.main != null && Vector3.Distance(Camera.main.transform.position, playPosition) > clipInfo.maxRange) return;

#if SOULLINK_USE_MASTERAUDIO
            if (!clipInfo.is3D)
            {
                MasterAudio.PlaySound(clipInfo.clip, clipInfo.volume);
            }
            else
            {
                MasterAudio.PlaySound3DAtVector3(clipInfo.clip, playPosition, clipInfo.volume);
            }
#else
            // Use the next audio source from the pool and use it here
            _audioSources[_audioSourceIndex].outputAudioMixerGroup = _outputAudioMixerGroup;
            _audioSources[_audioSourceIndex].transform.position = playPosition;
            _audioSources[_audioSourceIndex].clip = clipInfo.clip;
            _audioSources[_audioSourceIndex].volume = clipInfo.volume;
            _audioSources[_audioSourceIndex].spatialBlend = clipInfo.is3D ? 1f : 0f;
            _audioSources[_audioSourceIndex].minDistance = clipInfo.minRange;
            _audioSources[_audioSourceIndex].maxDistance = clipInfo.maxRange;
            _audioSources[_audioSourceIndex].Play();

            _audioSourceIndex++;
            if (_audioSourceIndex >= _maxAudioSources)
            {
                _audioSourceIndex = 0;
            } // if
#endif
        } // PlaySound()

        public Transform Spawn(Transform sourceObj, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (_poolManager != null)
            {
                var spawn = _poolManager.Spawn(sourceObj, position, rotation, parent, 1f, 1f, Vector3.one);
                if (spawn == null) return null;

                ISpawnable spawnable = spawn.gameObject.GetComponentInChildren<ISpawnable>();

                if (spawnable != null)
                {
                    spawnable.SetSpawnedBy(SpawnedBy.SoulLinkGlobal);
                }

                return spawn;
            }
            else
            {
                return Instantiate(sourceObj, position, rotation, null);
            }
        } // Spawn()

        public void Despawn(Transform spawn)
        {
            if (_poolManager != null)
            {
                _poolManager.Despawn(spawn);
            }
            else
            {
                // Just destroy spawn if no pooling solution is being used
                Destroy(spawn);
            }
        } // Despawn()
    } // class SoulLinkGlobal
} // namespace Magique.SoulLink
