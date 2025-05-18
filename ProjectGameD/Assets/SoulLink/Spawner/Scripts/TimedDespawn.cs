using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Timed Despawn")]
    public class TimedDespawn : MonoBehaviour, ISpawnEvents
    {
        [SerializeField]
        private float _despwnTime = 0f;

        private bool _spawned = false;

        /// <summary>
        /// When using PoolManager, this function automatically gets called when the instance is spawned. Otherwise, it is not used.
        /// </summary>
        public void OnSpawned()
        {
            _spawned = true;
            CancelInvoke();

            // Invoke despawn function on timed value
            Invoke("Despawn", _despwnTime);
        } // OnSpawned()

        virtual public void OnDespawned()
        {
            // do nothing
        } // OnDespawned()

        private void Start()
        {
            if (!_spawned)
            {
                // Invoke despawn function on timed value
                Invoke("Despawn", _despwnTime);
            } // if
        } //Start()

        /// <summary>
        /// When using PoolManager, this function automatically gets called when the instance is despawned. Otherwise, it is not used.
        /// </summary>
        private void Despawn()
        {
            CancelInvoke();

            if (_spawned)
            {
                var spawnable = GetComponent<ISpawnable>();
                if (spawnable != null && isActiveAndEnabled)
                {
                    spawnable.ForceDespawn();
                }
                else
                {
                    SoulLinkGlobal.Instance.Despawn(transform);
                }
                return;
            } // if

            // Just destroy this if no pooling solution is being used
            Destroy(gameObject);
        } // Despawn()
    } // class TimedDespawn
} // namespace Magique.SoulLink