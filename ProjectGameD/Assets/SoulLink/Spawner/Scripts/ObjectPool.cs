using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple object pool to manage prefab instances for spawning/despawning on demand

// Copyright 2020-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class ObjectPool : MonoBehaviour
    {
        public Transform Prefab
        {
            get { return _prefab; }
            set { _prefab = value; }
        }

        [SerializeField]
        private Transform _prefab;

        public int InitialQuantity
        {
            get { return _initialQuantity; }
            set { _initialQuantity = value; }
        }

        [SerializeField]
        private int _initialQuantity = 5;

        public bool Reusable
        { 
            get { return _reusable; } 
            set { _reusable = value; }
        }

        [SerializeField]
        private bool _reusable = true;

        public bool GrowCapacity
        {
            get { return _growCapacity; }
            set { _growCapacity = value; }
        }

        [SerializeField]
        private bool _growCapacity = false;

        public bool EnableNavMeshAgentOnSpawn
        {
            get { return _enableNavmeshAgentOnSpawn; }
            set { _enableNavmeshAgentOnSpawn = value; }
        }

        [SerializeField]
        private bool _enableNavmeshAgentOnSpawn = true;

        public bool HideInstancesInHierarchy
        { 
            get { return _hideInstancesInHierarchy;  } 
            set { _hideInstancesInHierarchy = value; }
        }

        [SerializeField]
        private bool _hideInstancesInHierarchy = false;

        public int PoolCount
        { get { return _pool != null ? _pool.Count : 0; } }

        public Transform ParentTransform
        { 
            get { return _parentTransform; } 
            set { _parentTransform = value; }
        }

        [SerializeField]
        private Transform _parentTransform;

        // Using a stack to push/pop items for use. If no items are on the stack then a new one must be instantiated if allowed.
        private Stack<Transform> _pool = new Stack<Transform>();

        /// <summary>
        /// CreatePool
        /// </summary>
        public void CreatePool(Transform parent)
        {
            _parentTransform = parent;

            // Create a pool of prefab instances            
            for (int i = 0; i < _initialQuantity; ++i)
            {
                _pool.Push(GenerateInstance());
            } // for i
        } // CreatePool()

        /// <summary>
        /// DestroyPool
        /// </summary>
        public void DestroyPool()
        {
            // Destroy all pooled instances
            while (_pool.Count > 0)
            {
                var instance = _pool.Pop();
                DestroyImmediate(instance.gameObject);
            } // while
        } // DestroyPool()

        /// <summary>
        /// Fetch
        /// </summary>
        /// <returns></returns>
        public Transform Fetch()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                {
                    var instance = _pool.Pop();
                    if (instance != null)
                    {
                        return instance;
                    }
                    else
                    {
                        Debug.LogError("ObjectPool instance is null. Did you call Destroy on your instance rather than despawning?");
                        return null;
                    }
                }
                else if (_growCapacity)
                {
                    return GenerateInstance();
                }
                else
                {
                    return null;
                }
            } // lock
        } // Fetch()

        /// <summary>
        /// Release
        /// </summary>
        /// <param name="instance"></param>
        public void Release(Transform instance)
        {
            instance.gameObject.SetActive(false);
            lock (_pool)
            {
                instance.SetParent(_parentTransform);
                _pool.Push(instance);
            } // lock
        } // Release()

        /// <summary>
        /// GenerateInstance
        /// </summary>
        /// <returns></returns>
        public Transform GenerateInstance()
        {
            var instance = Instantiate(_prefab);
            if (_hideInstancesInHierarchy)
            {
                instance.hideFlags = HideFlags.HideInHierarchy;
            } // if

            instance.transform.SetParent(_parentTransform);
            instance.gameObject.SetActive(false);

            return instance;
        } // GenerateInstance()
    } // class ObjectPool
} // namespace Magique.SoulLink