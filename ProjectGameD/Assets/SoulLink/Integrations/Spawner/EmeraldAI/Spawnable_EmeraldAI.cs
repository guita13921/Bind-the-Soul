#if EMERALD_AI_PRESENT
using EmeraldAI;
using UnityEngine.Events;
using EmeraldAI.Utility;
using static Magique.SoulLink.ShaderUtils;
using UnityEngine;
using System.Collections;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Spawnable Emerald AI")]
    public partial class Spawnable_EmeraldAI : Spawnable
    {

//        public class TestData
//        {
//            public string stringData;
//        }

        private EmeraldAISystem _aiSystem;
        private Vector3 _aiNamePosition = Vector3.one;
        private Vector3 _aiHealthBarPosition = Vector3.one;

        public override void Start()
        {
            // Magique Fix. Remove the following code if not using the fix.
            if (!_isInitialized && !_spawned)
            {
                _navMeshAgent.enabled = true;

                // Call user's custom triggered event handler
                if (_onAgentEnabledEventHandler != null)
                {
                    _onAgentEnabledEventHandler.Invoke();
                } // if

                InitializeEmeraldAI();
                StartEmeraldAI();
                SetVisible(_renderers, _materials);
            } // if
        } // Start()

        public override void Awake()
        {
            base.Awake();

            AddEventHandlers();

            // Save original UI values so they can be properly scaled
            _aiNamePosition = _aiSystem.AINamePos;
            _aiHealthBarPosition = _aiSystem.HealthBarPos;
        } // Awake()

        override public void OnSpawned()
        {
            base.OnSpawned();

            // Scale the UI elements so they match the new spawn scale
            Vector3 scaleFactor = new Vector3(transform.localScale.x * _originalScale.x, transform.localScale.y * _originalScale.y, transform.localScale.z * _originalScale.z);
            _aiSystem.AINamePos = new Vector3(_aiNamePosition.x * scaleFactor.x, _aiNamePosition.y * scaleFactor.y, _aiNamePosition.z * scaleFactor.z);
            _aiSystem.HealthBarPos = new Vector3(_aiHealthBarPosition.x * scaleFactor.x, _aiHealthBarPosition.y * scaleFactor.y, _aiHealthBarPosition.z * scaleFactor.z);

            StartCoroutine(WaitForExtraAIData());
        } // OnSpawned()

        /// <summary>
        /// Waits a frame until any extra saved AI data is passed to this spawn after spawning
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForExtraAIData()
        {
            yield return new WaitForEndOfFrame();

            if (_extraAIData != null)
            {
                // restore rotation
                transform.rotation = _extraAIData.Rotation;

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

        public override void EnableNavMeshAgent(bool value = true)
        {
            base.EnableNavMeshAgent(value);

            if (value)
            {            
                // Magique Fix. remove the following 2 lines if not using the fix.
                InitializeEmeraldAI();
                StartEmeraldAI();
            } // if
        } // EnableAgent()

        void InitializeEmeraldAI()
        {
            // Magique: Emerald AI fix for NavMeshAgent errors on Pooling solutions. Comment out the following lines if you are not using this fix.
            if (!_isInitialized)
            {
                var aiInitializer = GetComponent<EmeraldAIInitializer>();
                if (aiInitializer != null)
                {
                    aiInitializer.Initialize();
                } // if
                _isInitialized = true;
            } // if
        } // InitializeEmeraldAI()

        void StartEmeraldAI()
        {
            var aiSystem = GetComponent<EmeraldAISystem>();
            if (aiSystem != null)
            {
                if (_extraAIData != null)
                {
                    aiSystem.CurrentHealth = _extraAIData.HealthValue;
                }
                aiSystem.OnStartEvent.Invoke();
            } // if
        } // StartEmeraldAI()

        void AddEventHandlers()
        {
            _aiSystem = GetComponent<EmeraldAISystem>();
            _aiSystem.DeathEvent.AddListener(() => OnDeath());
        } // AddEventHandlers()

        bool EventHandlerAssigned(UnityEvent someEvent, string methodName)
        {
            int eventCount = someEvent.GetPersistentEventCount();
            if (eventCount == 0) return false;

            for (int i = 0; i < eventCount; i++)
            {
                if (someEvent.GetPersistentTarget(i) == this)
                {
                    if (someEvent.GetPersistentMethodName(i) == methodName)
                    {
                        return true;
                    } // if
                } // if
            } // for i

            return false;
        } // EventHandlerAssigned()

        override public bool AllowDespawn()
        {
            return _aiSystem.CombatStateRef != EmeraldAISystem.CombatState.Active || _allowDespawnInCombat;
        } // AllowDespawn()
    } // class Spawnable_EmeraldAI
} // namespace Magique.SoulLink
#endif