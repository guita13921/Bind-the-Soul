using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Magique.SoulLink
{
    public class FlexCollider : MonoBehaviour
    {
        public float Radius
        {
            get
            {
                if (_is3D) return _collider3D.radius;
                else return _collider2D.radius;
            }

            set
            {
                if (_is3D) _collider3D.radius = value;
                else _collider2D.radius = value;
            }
        }

        public bool IsTrigger
        {
            get
            {
                if (_is3D) return _collider3D.isTrigger;
                else return _collider2D.isTrigger;
            }

            set
            {
                if (_is3D) _collider3D.isTrigger = value;
                else _collider2D.isTrigger = value;
            }
        }

        public bool Enabled
        {
            get
            {
                if (_is3D) return _collider3D.enabled;
                else return _collider2D.enabled;
            }

            set
            {
                if (_is3D) _collider3D.enabled = value;
                else _collider2D.enabled = value;
            }
        }


        private SphereCollider _collider3D;
        private CircleCollider2D _collider2D;
        private bool _is3D = true;

        public void CreateCollider(bool is3D = true)
        {
            _is3D = is3D;

            if (_collider2D != null || _collider3D != null)
            {
                Debug.LogError("Error: Cannot recreate FlexCollider of a different kind.");
            } // if

            if (is3D)
            {
                _collider3D = gameObject.AddComponent<SphereCollider>();
            }
            else
            {
                _collider2D = gameObject.AddComponent<CircleCollider2D>();
            }
        } // CreateCollider()
    } // class FlexCollider
} // namespace Magique.SoulLink
