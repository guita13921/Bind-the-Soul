using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SpawnAction_Scaler
// A built-in spawn action for scaling spawns up/down
//
// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnAction_Scaler : BaseSpawnAction
    {
        [Tooltip("The minimum scale to start and end with.")]
        [SerializeField]
        private float _minScale = 0f;

        [Tooltip("How long to take for the spawn to fully scale up on spawn.")]
        [SerializeField]
        protected float _scaleUpLength = 2f;

        [Tooltip("How long to take for the spawn to fully scale down on despawn.")]
        [SerializeField]
        protected float _scaleDownLength = 2f;

        [Tooltip("Sink into the terrain on despawn.")]
        [SerializeField]
        private bool _sinkOnDespawn = true;

        [Tooltip("How deep to sink into the terrain.")]
        [SerializeField]
        private float _sinkDistance = 0.5f;

        private Vector3 _targetScale = Vector3.one;

        override public void FinalSpawnState()
        {
            transform.localScale = _targetScale;
        } // FinalSpawnState()

        override public void FinalDespawnState()
        {
            transform.localScale = Vector3.zero;
        } // FinalDespawnState()

        override public void PerformSpawnAction()
        {
            // Start at 0 scale and then scale up with coroutine until it reaches target scale
            _targetScale = transform.localScale;
            transform.localScale.Set(_minScale, _minScale, _minScale);

            StartCoroutine(ScaleUp());
        } // PerformSpawnAction()

        IEnumerator ScaleUp()
        {
            float timeElapsed = 0;
            float dx;

            while (timeElapsed < _scaleUpLength)
            {
                dx = Mathf.Lerp(_minScale, _targetScale.x, timeElapsed / _scaleUpLength);
                transform.localScale.Set(dx, dx, dx);
                timeElapsed += Time.deltaTime;

                yield return null;
            } // while

            SpawnActionComplete();
        } // ScaleUp()

        override public void PerformDespawnAction()
        {
            StartCoroutine(ScaleDown());
        } // PerformDespawnAction()

        IEnumerator ScaleDown()
        {
            float timeElapsed = 0;
            float dx; // for shrinking
            float dy; // for sinking
            float sy = transform.position.y; // for sinking start position

            while (timeElapsed < _scaleDownLength)
            {
                dx = Mathf.Lerp(_targetScale.x, _minScale, timeElapsed / _scaleDownLength);
                transform.localScale.Set(dx, dx, dx);

                if (_sinkOnDespawn)
                {
                    dy = Mathf.Lerp(sy, sy - _sinkDistance, timeElapsed / _scaleDownLength);
                    transform.position.Set(transform.position.x, dy, transform.position.z);
                } // if

                timeElapsed += Time.deltaTime;

                yield return null;
            } // while

            DespawnActionComplete();
        } // ScaleDown()

        override public float GetSpawnActionDuration()
        {
            return _scaleUpLength;
        } // GetSpawnActionDuration()

        override public float GetDespawnActionDuration()
        {
            return _scaleDownLength;
        } // GetDespawnActionDuration()
    } // class SpawnAction_Scaler
} // namespace Magique.SoulLink
