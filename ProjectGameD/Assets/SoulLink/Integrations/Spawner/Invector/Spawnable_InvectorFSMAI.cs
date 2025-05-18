#if INVECTOR_AI_TEMPLATE
using Invector;
using Invector.vCharacterController.AI;
using Invector.vCharacterController.AI.FSMBehaviour;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Invector FSM AI")]
    public partial class Spawnable_InvectorFSMAI : Spawnable
    {
        private vControlAICombat _aiSystem;

        /// <summary>
        /// Get a reference to the AI system so we can determine if it is in combat
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            _aiSystem = GetComponent<vControlAICombat>();
        } // Awake()

        override public void OnSpawned()
        {
            base.OnSpawned();

            StartCoroutine(WaitForExtraAIData());
        } // OnSpawned()

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
                var health = GetComponent<vHealthController>();
                if (health != null)
                {
                    health.ChangeHealth(_extraAIData.HealthValue);
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

        /// <summary>
        /// Performs custom reset code to put Invector FSM AI into proper state for next spawn
        /// </summary>
        override public void OnDespawned()
        {
            // Uncomment the following code if you are using Invector FSM AI version prior to 1.1.8b
//            var aiMotor = GetComponent<vAIMotor>();
//            if (aiMotor != null)
//            {
//                aiMotor.ResetDeathBehaviour();
//            }

            var health = GetComponent<vHealthController>();
            if (health != null)
            {
                health.ResetHealth();
            }

            var fsm = GetComponent<vFSMBehaviourController>();
            if (fsm != null)
            {
                fsm.ResetFSM();
            }

            base.OnDespawned();
        } // OnDespawned

        override public bool AllowDespawn()
        {
            return !_aiSystem.isInCombat;
        } // AllowDespawn()
    } // class Spawnable_InvectorFSMAI
} // namespace Magique.SoulLink
#endif
