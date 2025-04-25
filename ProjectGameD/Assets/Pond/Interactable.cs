using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;

        private void OrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {
            Debug.Log("You Interacted with an object");
        }
    }
}

