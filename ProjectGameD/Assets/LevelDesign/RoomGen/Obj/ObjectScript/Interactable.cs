using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string InteractableText;

        private void OawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {
            Debug.Log("You interacted with an oject!!");
        }
    }
}
