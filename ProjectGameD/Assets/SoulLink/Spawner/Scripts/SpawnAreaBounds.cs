using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnAreaBounds : MonoBehaviour
    {
        public Vector3 min
        {
            get { return (_renderer != null ? _renderer.bounds.min : Vector3.zero); }
        }

        public Vector3 max
        {
            get { return (_renderer != null ? _renderer.bounds.max : Vector3.zero); }
        }

        public Bounds bounds
        {
            get { return (_renderer != null ? _renderer.bounds : new Bounds()); }
        }

        private Renderer _renderer = null;

        void Start()
        {
            // Turn off rendering of this bounds object so it is invisible at run-time
            _renderer = gameObject.GetComponent<MeshRenderer>();
            if (_renderer != null)
            {
                _renderer.enabled = false;
            } // if
        } // Start()
    } // class SpawnAreaBounds
} // namespace Magique.SoulLink
