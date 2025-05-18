using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private string _tagToDestroy = "Enemy";

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;
        } // Awake()

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(_tagToDestroy))
            {
                GetComponent<TimedDespawn>().CancelInvoke();
                var spawnable = GetComponent<ISpawnable>();
                if (spawnable != null)
                {
                    spawnable.ForceDespawn();
                } // if
            } // if
        } // OnTriggerEnter2D()

        private void OnBecameVisible()
        {
            _collider.enabled = true;
        } // OnBecameVisible()

        private void OnBecameInvisible()
        {
            _collider.enabled = false;
        } // OnBecameInvisible()
    } // class Bullet
} // namespace Magique.SoulLink
