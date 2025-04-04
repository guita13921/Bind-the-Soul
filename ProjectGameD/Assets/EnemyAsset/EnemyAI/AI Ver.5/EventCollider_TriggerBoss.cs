using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EventCollider_TriggerBoss : MonoBehaviour
    {
        WorldEventManager worldEventManager;

        void Awake()
        {
            worldEventManager = FindObjectOfType<WorldEventManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                worldEventManager.ActivateBossFight();
            }
        }
    }
}