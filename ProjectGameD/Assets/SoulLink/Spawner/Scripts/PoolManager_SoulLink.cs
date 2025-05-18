using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PoolManager_SoulLink : PoolManager
    {
        public List<string> Categories
        { get { return _categories; } }

        [SerializeField]
        private List<string> _categories = new List<string>();

        public ObjectPoolDictionary ObjectPools
        {  get { return _objectPools; } }

        [SerializeField]
        private ObjectPoolDictionary _objectPools = new ObjectPoolDictionary();

        public PoolCategoryDictionary PoolCategories
        { get { return _poolCategories; } }

        [SerializeField]
        private PoolCategoryDictionary _poolCategories = new PoolCategoryDictionary();

        private Dictionary<Transform, Transform> _prefabRef = new Dictionary<Transform, Transform>();

        static public PoolManager_SoulLink Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            // Synchronize categories
            _categories.Clear();
            foreach (var category in _poolCategories)
            {
                _categories.Add(category.Key);
            } // foreach
        } // Awake()

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            foreach (var item in _objectPools)
            {
                if (item.Key != null)
                {
                    Debug.Log("Create object pool for " + item.Key.name);
                    item.Value.CreatePool(transform);
                }
                else
                {
                    Debug.LogError("Unable to create an object pool for an item with a missing reference. Did you remove the prefab from your project?");
                }
            } // foreach
        } // Start()

        /// <summary>
        /// Spawn
        /// </summary>
        /// <param name="itemPrefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parentTransform"></param>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        /// <param name="origScale"></param>
        /// <returns></returns>
        override public Transform Spawn(Transform itemPrefab, Vector3 position, Quaternion rotation, Transform parentTransform, float minScale, float maxScale, Vector3 origScale)
        {
            if (!_objectPools.ContainsKey(itemPrefab))
            {
                Debug.LogError("PoolManager could not spawn " + itemPrefab.name + " because it is not in any object pools.");
                return null;
            } // if

            var instance = _objectPools[itemPrefab].Fetch();
            if (instance != null)
            {
                var scale = (minScale != 1f || maxScale != 1f) ? Random.Range(minScale, maxScale) : 1f;
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.localScale = new Vector3(itemPrefab.localScale.x * scale, itemPrefab.localScale.y * scale, itemPrefab.localScale.x * scale);
                instance.transform.SetParent(parentTransform);
                instance.gameObject.SetActive(true);

                var spawnable = instance.GetComponentInChildren<ISpawnable>();
                if (spawnable != null)
                {
                    spawnable.SetOriginalScale(origScale);
                } // if

                // Send OnSpawned event to all eligible components on this instance
                var objsWithEvents = instance.GetComponentsInChildren<ISpawnEvents>();
                foreach (var obj in objsWithEvents)
                {
                    obj.OnSpawned();
                } // foreach

                _prefabRef[instance] = itemPrefab;

                AddActiveSpawn(itemPrefab, instance);

                // Inform the SpawnArea that a spawn was successfully added
                SpawnArea spawnArea = parentTransform != null ? parentTransform.gameObject.GetComponent<SpawnArea>() : null;
                if (spawnArea != null)
                {
                    spawnArea.SpawnAdded();
                } // if

                return instance;
            } // if

            Debug.LogError("PoolManager failed to spawn " + itemPrefab.name + " because it could not fetch an instance from any object pools.");
            return null;
        } // Spawn()

        /// <summary>
        /// Despawn
        /// </summary>
        /// <param name="itemPrefab"></param>
        /// <returns></returns>
        override public bool Despawn(Transform instance)
        {
            if (!_prefabRef.ContainsKey(instance))
            {
                Debug.LogError("PoolManager could not despawn " + instance.name + " because it was not spawned through PoolManager.");
                return false;
            } // if

            // Send OnDespawned event to all eligible components on this instance
            var objsWithEvents = instance.GetComponentsInChildren<ISpawnEvents>();
            foreach (var obj in objsWithEvents)
            {
                obj.OnDespawned();
            } // foreach

            RemoveActiveSpawn(_prefabRef[instance], instance);

            if (_objectPools[_prefabRef[instance]].Reusable)
            {
                _objectPools[_prefabRef[instance]].Release(instance);
            }
            else
            {
                // Cannot re-use this so just destroy it
                Destroy(instance.gameObject);
            }
			
            return true;
        } // Despawn()

        /// <summary>
        /// IsInPoolManager
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        override public bool IsInPoolManager(Transform prefab)
        {
            return _objectPools.ContainsKey(prefab);
        } // IsInPoolManager()

        /// <summary>
        /// AddPoolItem
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="populationCap"></param>
        /// <param name="database"></param>
        override public void AddPoolItem(Transform prefab, int populationCap, SoulLinkSpawnerDatabase database, bool reusable = true, bool growCapacity = false)
        {
            if (prefab == null || database == null) return;

            // Add it to PoolManager if it doesn't already exist; else update the values as needed
            if (!IsInPoolManager(prefab))
            {
                var tempObj = new GameObject("TempObj");
                var objPool = tempObj.AddComponent<ObjectPool>();
                objPool.Prefab = prefab;
                objPool.InitialQuantity = populationCap;
                objPool.Reusable = reusable;
                objPool.GrowCapacity = growCapacity;
                objPool.hideFlags = HideFlags.HideInHierarchy;

                // Automatically determine if the nav mesh agent needs to be enabled or not based on the prefab's current component setting
                var navMeshAgent = prefab.GetComponent<NavMeshAgent>();
                if (navMeshAgent != null)
                {
                    objPool.EnableNavMeshAgentOnSpawn = !navMeshAgent.enabled;
                } // if

                var newObjPool = Instantiate(tempObj, transform);
                newObjPool.name = prefab.name;
                _objectPools[prefab] = newObjPool.GetComponent<ObjectPool>();
                newObjPool.hideFlags = HideFlags.HideInHierarchy;

                DestroyImmediate(tempObj);

                var categoryName = SPAWNER_CATEGORY_PREFIX + database.name;
                AddCategory(categoryName);

                // Put into pool dictionary so we have a reference by category
                _poolCategories[categoryName][prefab] = _objectPools[prefab];
            }
            else
            {
                UpdatePoolItem(prefab, populationCap, reusable);
            }
        } // AddPoolItem()

        /// <summary>
        /// UpdatePoolItem
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="populationCap"></param>
        override public void UpdatePoolItem(Transform prefab, int populationCap, bool reusable = true)
        {
            if (!_objectPools.ContainsKey(prefab)) return;

            // Always use the highest defined population cap
            if (_objectPools[prefab].InitialQuantity < populationCap)
            {
                _objectPools[prefab].InitialQuantity = populationCap;
            } // if
			
            _objectPools[prefab].Reusable = reusable;
        } // UpdatePoolItem()

        /// <summary>
        /// Cleanup
        /// </summary>
        override public void Cleanup()
        {
            // For editor use only so just return if the app is playing
            if (Application.isPlaying) return;

            ObjectPools.Cleanup();
        } // Cleanup()

        /// <summary>
        /// ClearPoolItems
        /// </summary>
        /// <param name="database"></param>
        /// <param name="allItems"></param>
        override public void ClearPoolItems(SoulLinkSpawnerDatabase database, bool allItems = false)
        {
            // For editor use only so just return if the app is playing
            if (Application.isPlaying || database == null) return;

            var categoryName = SPAWNER_CATEGORY_PREFIX + database.name;

            // First remove all the instances of ObjectPool
            var objPools = gameObject.GetComponentsInChildren<ObjectPool>();
            foreach (var objPool in objPools)
            {
                foreach (var category in _poolCategories)
                {
                    if (category.Key == categoryName || allItems == true)
                    {
                        foreach (var pool in category.Value)
                        {
                            if (pool.Key == objPool.Prefab)
                            {
                                DestroyImmediate(objPool.gameObject);
                            } // if
                        } // foreach
                    } // if
                } // foreach
            } // foreach

            var removeList = new List<Transform>();
            if (_poolCategories.ContainsKey(categoryName))
            {
                foreach (var objPool in _objectPools)
                {
                    for (int i = 0; i < _poolCategories[categoryName].Values.Count; ++i)
                    {
                        if (_poolCategories[categoryName].ContainsKey(objPool.Key))
                        {
                            removeList.Add(objPool.Key);
                        } // if
                    } // for i
                } // foreach

                foreach (var item in removeList)
                {
                    _objectPools.Remove(item);
                } // foreach

                // Remove all items from poolCategories that are in SPAWNER_CATEGORY_NAME
                _poolCategories.Remove(categoryName);
            }
            else if (allItems)
            {
                _objectPools.Clear();
                _poolCategories.Clear();
            }
        } // ClearPoolItems()

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="categoryName"></param>
        override public void AddCategory(string categoryName)
        {
            // Create the category if not there
            if (!_poolCategories.ContainsKey(categoryName))
            {
                _poolCategories[categoryName] = new ObjectPoolDictionary();
            } // if
        } // AddCategory()

        /// <summary>
        /// Remove an existing category and all its object pools
        /// </summary>
        /// <param name="categoryName"></param>
        override public bool RemoveCategory(string categoryName)
        {
            var removeList = new List<Transform>();
            if (_poolCategories.ContainsKey(categoryName))
            {
                foreach (var objPool in _objectPools)
                {
                    for (int i = 0; i < _poolCategories[categoryName].Values.Count; ++i)
                    {
                        if (_poolCategories[categoryName].ContainsKey(objPool.Key))
                        {
                            removeList.Add(objPool.Key);
                        } // if
                    } // for i
                } // foreach

                foreach (var item in removeList)
                {
                    _objectPools.Remove(item);
                } // foreach

                // Remove the category
                _poolCategories.Remove(categoryName);
                _categories.Remove(categoryName);

                return true;
            } // if

            if (_categories.Contains(categoryName))
            {
                _categories.Remove(categoryName);
                return true;
            } // if

            return false;
        } // RemoveCategory()

        /// <summary>
        /// Returns whether or not a category exists in PoolManager
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        override public bool CategoryExists(string categoryName)
        {
            return _categories.Contains(categoryName);
        } // CategoryExists()
    } // class PoolManager
} //namespace Magique.SoulLink

