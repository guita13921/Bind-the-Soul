#if SOULLINK_USE_MAC
using UnityEngine;

using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Controller;
using System.Collections;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Spawnable_MAC : Spawnable
    {
        [SerializeField]
        private bool _autoCreateWanderArea = true;

        [SerializeField]
        private float _wanderRadius = 30f;

        private AIWanderArea _wanderArea;
        private MAnimalAIControl _aiControl;
        private MAnimal _animal;
        private MAnimalBrain _brain;
        private Stats _statsManager;

        /// <summary>
        /// Initial setup for this AI
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            _aiControl = GetComponentInChildren<MAnimalAIControl>();
            _animal = GetComponentInChildren<MAnimal>();
            _brain = GetComponentInChildren<MAnimalBrain>();
            _statsManager = GetComponentInChildren<Stats>();

            DisableAnimalControl();
        } // Awake()

        public override void Start()
        {
            base.Start();

            if (!_spawned)
            {
                CreateWanderArea();
                EnableAnimalControl();
            } // if
        } // Start()

        /// <summary>
        /// Automatically called when the AI spawns
        /// </summary>
        override public void OnSpawned()
        {
            base.OnSpawned();

            if (_statsManager != null)
            {
                var health = _statsManager.Stat_Get("Health");
                health.SetActive(true);
                health.Reset_to_Max();
            } // if

            EnableAnimalControl();
            CreateWanderArea();

            StartCoroutine(WaitForExtraAIData());
        } // OnSpawned()

        /// <summary>
        /// Enable the animal component and put it in the proper state
        /// </summary>
        void EnableAnimalControl()
        {
            if (_animal != null)
            {
                _animal.State_Activate(StateEnum.Idle);
                _animal.OnStateChange.AddListener(OnAIStateChanged);
                _animal.enabled = true;
            } // if

            if (_aiControl != null)
            {
                _aiControl.gameObject.SetActive(true);
            } // if

            if (_brain != null)
            {
                _brain.SetEnable(true);
                _brain.StartBrain();
            } // if
        } // EnableAnimalControl()

        /// <summary>
        /// Disable the animal component and ai control
        /// </summary>
        void DisableAnimalControl()
        {
            if (_animal != null)
            {
                _animal.enabled = false;
            } // if

            if (_aiControl != null)
            {
                _aiControl.gameObject.SetActive(false);
            } // if
        } // DisableAnimalControl()

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
                // Restore rotation
                transform.rotation = _extraAIData.Rotation;

                // Restore health
                if (_statsManager != null)
                {
                    var health = _statsManager.Stat_Get("Health");
                    health.Value = _extraAIData.HealthValue;
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
        /// Automatically called when the AI despawns
        /// </summary>
        override public void OnDespawned()
        {
            if (_animal != null)
            {
                _animal.OnStateChange.RemoveListener(OnAIStateChanged);
                _animal.enabled = false;
            } // if

            if (_aiControl != null)
            {
                _aiControl.gameObject.SetActive(false);
            } // if

            DestroyWanderArea();

            base.OnDespawned();
        } // OnDespawned()

        /// <summary>
        /// Creates an AI wander area at run-time so the AI can wander in the game world
        /// </summary>
        public void CreateWanderArea()
        {
            if (!_autoCreateWanderArea) return;

            var gameObj = new GameObject("WanderArea");
            _wanderArea = gameObj.AddComponent<AIWanderArea>();
            _wanderArea.transform.position = gameObject.transform.position;
            _wanderArea.radius = _wanderRadius;

            // Assign to malbers target
            if (_aiControl != null)
            {
                _aiControl.SetTarget(_wanderArea.transform);
            } // if
        } // CreateWanderArea()

        /// <summary>
        /// Destroys the AI wander area when no longer needed
        /// </summary>
        public void DestroyWanderArea()
        {
            if (!_autoCreateWanderArea) return;

            if (_aiControl != null)
            {
                _aiControl.ClearTarget();
                _aiControl.StopWait();
            } // if

            if (_brain)
            {
                _brain.LastWayPoint = null;
            } // if

            Destroy(_wanderArea.gameObject);
        } // DestroyWanderArea()

        /// <summary>
        /// Monitor the AI's state to know when it has died
        /// </summary>
        /// <param name="state"></param>
        public void OnAIStateChanged(int state)
        {
            if (state == StateEnum.Death)
            {
                OnDeath();

                if (!_spawned && _autoCreateWanderArea)
                {
                    DestroyWanderArea();
                } // if
            } // if
        } // OnAIStateChanged()
    } // Spawnable_MAC()
} // namespace Magique.SoulLink
#endif