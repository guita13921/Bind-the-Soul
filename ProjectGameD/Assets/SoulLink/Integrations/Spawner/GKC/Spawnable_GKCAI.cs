#if SOULLINK_USE_GKC
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Magique.SoulLink
{
    public class Spawnable_GKCAI : Spawnable
    {
        public override void Awake()
        {
            base.Awake();

            AddEventHandlers();
        } // Awake()

        override public void OnSpawned()
        {
            base.OnSpawned();

            // TODO: ResetAI for future use
            //ResetAI();

            StartCoroutine(WaitForExtraAIData());
        } // OnSpawned()


        override public void OnDeath()
        {
            base.OnDeath();

            _isDead = true;
        }

        /// <summary>
        /// Waits a frame until any extra saved AI data is passed to this spawn after spawning
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForExtraAIData()
        {
            yield return new WaitForEndOfFrame();

            // Restore animator state
            if (_extraAIData != null)
            {
                // restore rotation
                transform.rotation = _extraAIData.Rotation;

                // restore health
                var health = GetComponentInChildren<health>();
                if (health != null)
                {
                    health.healthAmount = _extraAIData.HealthValue;
                } // if

                // Restore animator state
                var animator = GetComponent<Animator>();
                if (animator != null)
                {
                    for (int i = 0; i < _extraAIData.AnimLayerData.Count; ++i)
                    {
                        animator.Play(_extraAIData.AnimLayerData[i].hash, i);
                    } // for i
                } // if

                DeserializeData(_extraAIData);
            } // if
        } // WaitForExtraAIData()

        void AddEventHandlers()
        {
            var health = GetComponentInChildren<health>();
            if (health)
            {
                health.deadFuncionCall.AddListener(() => OnDeath());
            } // if
        } // AddEventHandlers()

        /// <summary>
        /// Performs custom reset code to put Invector FSM AI into proper state for next spawn
        /// </summary>
        override public void OnDespawned()
        {
            base.OnDespawned();
        } // OnDespawned

        void ResetAI()
        {
            if (!_isDead) return;

            var health = GetComponentInChildren<health>();
            if (health != null)
            {
                health.resurrect();
                health.eventToResurrectAfterDelay.Invoke();
            }

            var findObjectives = GetComponentInChildren<findObjectivesSystem>();
            if (findObjectives != null)
            {
                findObjectives.wanderEnabled = true;
            }

            GetComponentInChildren<NavMeshAgent>().enabled = true;

            _isDead = false;
        }

        override public bool AllowDespawn()
        {
            var findObj = GetComponentInChildren<findObjectivesSystem>();
            return (findObj != null && findObj.isOnSpotted());
        } // AllowDespawn()
    } // class Spawnable_GKCAI
} // namespace Magique.SoulLink
#endif