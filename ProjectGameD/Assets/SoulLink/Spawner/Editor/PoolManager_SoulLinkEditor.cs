using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(PoolManager_SoulLink), true)]
    public class PoolManager_SoulLinkEditor : Editor
    {
        private PoolManager_SoulLink _myTarget;
        private SerializedObject _mySerializedObject;

        // bool

        // float

        // int

        // string

        // enum

        // object

        private string _newCategoryName = "Unanmed Category";
        private List<string> _selectableCategories = new List<string>();
        private List<string> _nonSelectableCatgories = new List<string>();
        private int _categorySelected = 0;
        private Transform _newPrefab;
        static private Dictionary<string, bool> ShowPoolData = new Dictionary<string, bool>();

        private void OnEnable()
        {
            _myTarget = (PoolManager_SoulLink)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // Clear all database Spawner_ items since they should not exist at edit time anymore
            SpawnRegion[] regions = Resources.FindObjectsOfTypeAll<SpawnRegion>() as SpawnRegion[];
            foreach (var region in regions)
            {
                _myTarget.ClearPoolItems(region.Database);
            } // foreach

            var spawner = FindObjectOfType<SoulLinkSpawner>();
            if (spawner != null)
            {
                _myTarget.ClearPoolItems(spawner.Database);
            } // if

            // One time init
            InitCategories();
            InitSelectableCategories();


            // bool property initializers

            // float property initializers

            // int property initializers

            // string property initializers

            // enum property initializers

            // object property initializers
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying && SoulLinkSpawner.Instance != null && (SoulLinkSpawner.Instance.Initializing || SoulLinkSpawner.Quitting)) return;

            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            //DrawDefaultInspector();
            DrawPoolCategoriesGUI();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the PoolManager_SoulLink into Undo
                Undo.RecordObject(_myTarget, "PoolManager_SoulLink Editor Modify");

                EditorUtility.SetDirty(_myTarget);

                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void InitCategories()
        {
            // Synchronize categories
            _myTarget.Categories.Clear();
            foreach (var category in _myTarget.PoolCategories)
            {
                _myTarget.Categories.Add(category.Key);
            } // foreach
        } // InitCategories()

        void InitSelectableCategories()
        {
            _selectableCategories.Clear();
            _selectableCategories.AddRange(_myTarget.Categories.FindAll(e => !e.Contains(SPAWNER_CATEGORY_PREFIX)));

            foreach (var category in _selectableCategories)
            {
                if (!ShowPoolData.ContainsKey(category))
                {
                    ShowPoolData[category] = false;
                } // if
            } // foreach

            _nonSelectableCatgories.Clear();
            _nonSelectableCatgories.AddRange(_myTarget.Categories.FindAll(e => e.Contains(SPAWNER_CATEGORY_PREFIX)));

            foreach (var category in _nonSelectableCatgories)
            {
                if (!ShowPoolData.ContainsKey(category))
                {
                    ShowPoolData[category] = true;
                } // if
            } // foreach
        } // InitSelectableCategories()

        void DrawPoolCategoriesGUI()
        {
            string errorText = "";

            InitSelectableCategories();

            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Category");
            _newCategoryName = EditorGUILayout.TextField(_newCategoryName);
            bool buttonAdd = GUILayout.Button("+", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            if (buttonAdd)
            {
                if (!_myTarget.Categories.Contains(_newCategoryName))
                {
                    _myTarget.Categories.Add(_newCategoryName);
                }
                else
                {
                    errorText = "Category " + _newCategoryName + " already exists.";

                    // Display error popup dialog if necessary
                    if (errorText != string.Empty)
                    {
                        if (EditorUtility.DisplayDialog(
                            "Error",
                            errorText,
                            "OK"))
                        {
                        } // if
                    } // if
                }
            } // if
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            // Add new prefab pool by first selecting a category. Note that SPAWNER_CATEGORY_NAME is not allowed
            EditorUtils.BeginBoxStyleDark();

            EditorGUILayout.BeginHorizontal();
            _categorySelected = EditorGUILayout.Popup("Category Name", _categorySelected, _selectableCategories.ToArray());
            bool buttonRemoveCategory = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            if (buttonRemoveCategory)
            {
                if (EditorUtility.DisplayDialog(
                    "Warning",
                    "Removing a category will remove all the items in that object pool. Do you want to continue?",
                    "OK", "CANCEL"))
                {
                    // Remove the category and all object pools in that category
                    if (_myTarget.RemoveCategory(_selectableCategories[_categorySelected]))
                    {
                        _categorySelected = _selectableCategories.Count > 0 ? 0 : -1;
                    } // if
                } // if
                GUIUtility.ExitGUI(); // to prevent inspector draw issues
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _newPrefab = EditorGUILayout.ObjectField("Prefab", _newPrefab, typeof(Transform), false) as Transform;
            bool buttonAddPrefab = GUILayout.Button("+", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            if (buttonAddPrefab)
            {
                // Make sure prefab is not already in any category
                if (!_myTarget.ObjectPools.ContainsKey(_newPrefab))
                {
                    AddPrefabPool(_selectableCategories[_categorySelected], _newPrefab);
                    _newPrefab = null; // clear for next usage
                }
                else
                {
                    errorText = "Prefab " + _newPrefab.name + " already exists in PoolManager.";
                }

                // Display error popup dialog if necessary
                if (errorText != string.Empty)
                {
                    if (EditorUtility.DisplayDialog(
                        "Error",
                        errorText,
                        "OK"))
                    {
                    } // if
                } // if
            } // if
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Select a category and a prefab and then click + to add a new object pool.", MessageType.Info);

            EditorGUILayout.EndVertical();

            if (ShowPoolData.Count > 0 && (SoulLinkSpawner.Instance == null || !SoulLinkSpawner.Instance.Initializing))
            {
                EditorGUI.indentLevel++;
                foreach (var category in _myTarget.PoolCategories)
                {
                    if (ShowPoolData.ContainsKey(category.Key))
                    {
                        ShowPoolData[category.Key] = EditorGUILayout.Foldout(ShowPoolData[category.Key], category.Key + "(" + category.Value.Count + ")", true);
                        if (ShowPoolData[category.Key])
                        {
                            DrawCategoryItems(category);
                        } // if
                    } // if
                } // foreach
                EditorGUI.indentLevel--;
            } // if

/*            bool buttonRefresh = GUILayout.Button("Refresh Spawner", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonRefresh)
            {
                _myTarget.ClearPoolItems();
            } // if

            bool buttonCleanup = GUILayout.Button("Cleanup", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonCleanup)
            {
                _myTarget.Cleanup();
            } // if
*/
        } // DrawPoolCategoriesGUI()

        void AddPrefabPool(string categoryName, Transform prefab)
        {
            lock (_myTarget.ObjectPools)
            {
                var tempObj = new GameObject("TempObj");
                var objPool = tempObj.AddComponent<ObjectPool>();
                objPool.Prefab = prefab;

                var newObjPool = Instantiate(tempObj, _myTarget.transform);
                newObjPool.name = prefab.name;
                _myTarget.ObjectPools[prefab] = newObjPool.GetComponent<ObjectPool>();

                newObjPool.hideFlags = HideFlags.HideInHierarchy;

                DestroyImmediate(tempObj);

                lock (_myTarget.PoolCategories)
                {
                    // Create the category if not there
                    if (!_myTarget.PoolCategories.ContainsKey(categoryName))
                    {
                        _myTarget.PoolCategories[categoryName] = new ObjectPoolDictionary();
                    } // if

                    // Put into pool dictionary so we have a reference by category
                    _myTarget.PoolCategories[categoryName][prefab] = _myTarget.ObjectPools[prefab];
                } // lock
            } // lock
        } // AddPrefabPool()

        void DrawCategoryItems(KeyValuePair<string, ObjectPoolDictionary> category)
        {
            List<Transform> removeList = new List<Transform>();

            // Insure that items in the Spawner category are read-only since these are controlled by Spawner and should not be tampered with here
            GUI.enabled = !(category.Key.Contains(SPAWNER_CATEGORY_PREFIX));
            foreach (var group in category.Value)
            {
                bool deleteItem = DrawObjectPoolGUI(group.Value, GUI.enabled);
                if (deleteItem)
                {
                    removeList.Add(group.Key);
                } // if
            }
            GUI.enabled = true;

            foreach (var prefab in removeList)
            {
                RemoveObjectPool(prefab);
            } // foreach
        } // DrawCategoryItems()

        void RemoveObjectPool(Transform prefab)
        {
            // Destroy in hierarchy first
            DestroyImmediate(_myTarget.ObjectPools[prefab].gameObject);

            _myTarget.ObjectPools.Remove(prefab);

            foreach (var obj in _myTarget.PoolCategories)
            {
                obj.Value.Remove(prefab);
            } // foreach
        } // RemoveObjectPool()

        bool DrawObjectPoolGUI(ObjectPool objPool, bool allowDelete)
        {
            if (objPool == null || objPool.Prefab == null) return false;

            bool deleteItem = false;
            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(objPool.Prefab.name);
            if (allowDelete)
            {
                deleteItem = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            } // if
            EditorGUILayout.EndHorizontal();

            objPool.Prefab = EditorGUILayout.ObjectField("Prefab", objPool.Prefab, typeof(Transform), false) as Transform;
            objPool.InitialQuantity = EditorGUILayout.IntField("Initial Quantity", objPool.InitialQuantity);
            objPool.GrowCapacity = EditorGUILayout.Toggle("Grow Capacity", objPool.GrowCapacity);
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            return deleteItem;
        } // DrawObjectPoolGUI()
    } // PoolManager_SoulLinkEditor
} // namespace Magique.SoulLink
