using UnityEngine;
using UnityEngine.Events;

// BaseSpawnAction
// A base class for performing spawn/despawn actions

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BaseSpawnAction : MonoBehaviour, ISpawnAction
    {
        public bool DespawnActionEnabled
        { 
            get { return _despawnActionEnabled; } 
            set { _despawnActionEnabled = value; }
        }

        [SerializeField]
        protected bool _spawnActionEnabled = true;

        public bool SpawnActionEnabled
        {
            get { return _spawnActionEnabled; }
            set { _spawnActionEnabled = value; }
        }

        [SerializeField]
        protected bool _despawnActionEnabled = true;

        public UnityEvent OnSpawnActionStarted;
        public UnityEvent OnSpawnActionComplete;

        public UnityEvent OnDespawnActionStarted;
        public UnityEvent OnDespawnActionComplete;

        protected bool _performing = false;

        virtual public void SpawnActionComplete()
        {
            _performing = false;

            if (OnSpawnActionComplete != null)
            {
                OnSpawnActionComplete.Invoke();
            } // if
        } // SpawnActionComplete()

        virtual public void DespawnActionComplete()
        {
            if (OnDespawnActionComplete != null)
            {
                OnDespawnActionComplete.Invoke();
            } // if

            _performing = false;
        } // DespawnActionComplete()

        virtual public void FinalSpawnState()
        {
            throw new System.NotImplementedException();
        } // FinalSpawnState()

        virtual public void FinalDespawnState()
        {
            throw new System.NotImplementedException();
        } // FinalDespawnState()

        virtual public void PerformSpawnAction()
        {
            throw new System.NotImplementedException();
        } // PerformSpawnAction()

        virtual public void PerformDespawnAction()
        {
            throw new System.NotImplementedException();
        } // PerformDespawnAction()

        virtual public float GetSpawnActionDuration()
        {
            return 0f;
        } // GetSpawnActionDuration()

        virtual public float GetDespawnActionDuration()
        {
            return 0f;
        } // GetDespawnActionDuration()

        public bool IsPerforming()
        {
            return _performing;
        } // IsPerforming()
    } // class BaseSpawnAction
} // namespace Magique.SoulLink