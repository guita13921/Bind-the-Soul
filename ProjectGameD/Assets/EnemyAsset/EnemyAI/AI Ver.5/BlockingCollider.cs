using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class BlockingCollider : MonoBehaviour
    {
        BoxCollider blockingCollider;

        public float blockingColliderDamageAbsorption;

        private void Awake()
        {
            blockingCollider = GetComponent<BoxCollider>();
        }

        public void SetColliderDamageAbsorption(WeaponItem weapon)
        {
            if (weapon == null)
            {
                blockingColliderDamageAbsorption = weapon.physicalDamageAbsorption;
            }
        }

        public void EnableBlockingCollider()
        {
            blockingCollider.enabled = true;
        }

        public void DisableBlockingCollider()
        {
            blockingCollider.enabled = false;
        }
    }
}