using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveTracker : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _waveStartedSound;

        [SerializeField]
        private AudioClip _waveCompletedSound;

        [SerializeField]
        private AudioClip _waveItemRepeatStartedSound;

        [SerializeField]
        private AudioClip _waveItemRepeatCompletedSound;

        [SerializeField]
        private bool _debugLog = false;

        public void OnWaveStarted()
        {
            var sysMessage = string.Format("Wave {0} Starting", GetComponent<WaveManager>().WaveNumber);
            SpawnTrackerManager.Instance.SetSystemMessage(sysMessage, new SoundData() { audioClip = _waveStartedSound, soundIs3D = false });

            if (_debugLog)
            {
                Debug.Log(sysMessage);
            } // if
        } // OnWaveStarted()

        public void OnWaveComplete()
        {
            var sysMessage = string.Format("Wave {0} Completed", GetComponent<WaveManager>().WaveNumber);
            SpawnTrackerManager.Instance.SetSystemMessage(sysMessage, new SoundData() { audioClip = _waveCompletedSound, soundIs3D = false });

            if (_debugLog)
            {
                Debug.Log(sysMessage);
            } // if
        } // OnWaveComplete()

        public void OnWaveItemRepeatStarted()
        {
            SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _waveItemRepeatStartedSound, soundIs3D = false }, Vector3.zero);
        } // OnWaveItemRepeatStarted()

        public void OnWaveItemRepeatComplete()
        {
            SpawnTrackerManager.Instance.RepeatCounter++;
            SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _waveItemRepeatCompletedSound, soundIs3D = false }, Vector3.zero);
        } // OnWaveItemRepeatComplete()
    } // class WaveTracker
} // namespace Magique.SoulLink