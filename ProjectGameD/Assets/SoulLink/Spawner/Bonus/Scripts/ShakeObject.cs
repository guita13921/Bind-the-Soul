using System.Collections;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class ShakeObject : MonoBehaviour
    {
        public float ShakeMagnitude
        {
            get { return _shakeMagnitude; }
            set { _shakeMagnitude = value; }
        }

        [Tooltip("Adjust this value up to have a more noticeable shaking effect.")]
        [SerializeField]
        private float _shakeMagnitude = 3f;

        private bool _isShaking = false;

        public void Shake()
        {
            if (!_isShaking) StartCoroutine(PerformShaking());
        } // Shake()

        IEnumerator PerformShaking()
        {
            _isShaking = true;

            int shakeCount = 0;
            Vector3 savePosition = transform.position;

            while (shakeCount < 5)
            {
                yield return null;

                // randomize shake position
                float xOffset = Random.Range(0.01f, 0.025f) * _shakeMagnitude;
                float zOffset = Random.Range(0.01f, 0.025f) * _shakeMagnitude;
                if (Random.Range(0f, 100f) < 50f) xOffset *= -1f;
                if (Random.Range(0f, 100f) < 50f) zOffset *= -1f;
                transform.position =
                    new Vector3(transform.position.x + xOffset,
                                transform.position.y,
                                transform.position.z + zOffset);

                yield return null;

                // return to original position
                transform.position = savePosition;

                ++shakeCount;
            } // while

            _isShaking = false;
        } // PerformShaking()
    } // class ShakeObject
} // namespace Magique.SoulLink