using System.Collections;
using UnityEngine;

namespace Magique.SoulLink
{
    public class WaveWarning : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _warningSound;

        [SerializeField]
        private float _waveDelay;

        [SerializeField]
        private AudioClip _waveStartSound;

        public void OnWaveStarted()
        {
            SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _warningSound, soundIs3D = false }, Vector3.zero);
            Invoke("WaveStartSound", _waveDelay);
        }

        void WaveStartSound()
        {
            SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _waveStartSound, soundIs3D = false }, Vector3.zero);
        }
    }
}
