using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.ShaderUtils;

// SpawnAction_Dissolver
// A custom spawn action for dissolving and materializing spawns
//
// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnAction_Dissolver : BaseSpawnAction
    {
        public string DissolveKeyword
        {
            get { return _dissolveKeyword; }
            set { _dissolveKeyword = value; }
        }

        [Tooltip("Use the shader keyword that controls the dissolve amount.")]
        [SerializeField]
        protected string _dissolveKeyword = "_DissolveAmount";

        [Tooltip("How long to take for the spawn to fully materialize.")]
        [SerializeField]
        protected float _materializeLength = 2f;

        [Tooltip("How long to delay the dissolve after starting the despawn action.")]
        [SerializeField]
        private float _dissolveDelay = 0f;

        [Tooltip("How long to take for the spawn to dissolve.")]
        [SerializeField]
        protected float _dissolveLength = 2f;

        protected Dictionary<Material, Material> _materials = new Dictionary<Material, Material>();
        protected List<Renderer> _renderers = new List<Renderer>();

        /// <summary>
        /// Prepare this object for dissolving
        /// </summary>
        void Awake()
        {
            // Get all the materials in this object so we can control the alpha fading
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    _renderers.Add(renderer);

                    foreach (var material in renderer.materials)
                    {
                        if (material.HasProperty(_dissolveKeyword))
                        {
                            _materials[material] = new Material(material);
                        } // if
                    } // foreach material
                } // if
            } // foreach renderer
        } // Awake()

        public void SetDissolveKeyword(string value)
        {
            _dissolveKeyword = value;
        }

        public string GetDissolveKeyword()
        {
            return _dissolveKeyword;
        }

        public void SetMaterializeLength(float value)
        {
            _materializeLength = value;
        } // SetMaterializeLength()

        public float GetMaterializeLength()
        {
            return _materializeLength;
        } // GetMaterializeLength()

        public void SetDissolveLength(float value)
        {
            _dissolveLength = value;
        } // SetDissolveLength()

        public float GetDissolveLength()
        {
            return _dissolveLength;
        } // GetDissolveLength()

        override public void PerformSpawnAction()
        {
            if (_performing) return;

            if (OnSpawnActionStarted != null)
            {
                OnSpawnActionStarted.Invoke();
            } // if

            if (_spawnActionEnabled)
            {
                _performing = true;

                Materialize();
            }
            else
            {
                SpawnActionComplete();
            }
        } // PerformSpawnAction()

        override public void PerformDespawnAction()
        {
            if (_performing) return;

            if (OnDespawnActionStarted != null)
            {
                OnDespawnActionStarted.Invoke();
            } // if

            if (_despawnActionEnabled)
            {
                _performing = true;

                Dissolve();
            }
            else
            {
                DespawnActionComplete();
            }
        } // PerformSpawnAction()

        /// <summary>
        /// Materialize the spawn
        /// </summary>
        void Materialize()
        {
            foreach (var renderer in _renderers)
            {
                renderer.enabled = true;
            }

            var i = 0;
            foreach (var material in _materials)
            {
                // Start dissolve 1
                material.Key.SetFloat(_dissolveKeyword, 1);

                if (i == _materials.Count - 1)
                {
                    StartCoroutine(DoMaterialize(material.Key, _dissolveKeyword, _materializeLength, true));
                }
                else
                {
                    StartCoroutine(DoMaterialize(material.Key, _dissolveKeyword, _materializeLength));
                }
                ++i;
            } // foreach
        } // Materialize()

        /// <summary>
        /// Materialize the spawn's materials
        /// </summary>
        /// <param name="material"></param>
        /// <param name="dissolveKeyword"></param>
        /// <param name="duration"></param>
        /// <param name="callCompleteHandler"></param>
        /// <returns></returns>
        IEnumerator DoMaterialize(Material material, string dissolveKeyword, float duration, bool callCompleteHandler = false)
        {
            float timeElapsed = 0;
            float dissolve;

            while (timeElapsed < duration)
            {
                dissolve = Mathf.Lerp(1f, 0f, timeElapsed / duration);
                material.SetFloat(dissolveKeyword, dissolve);
                timeElapsed += Time.deltaTime;

                yield return null;
            } // while

            // Snap to final value
            dissolve = 0f;
            material.SetFloat(dissolveKeyword, dissolve);

            if (callCompleteHandler)
            {
                SpawnActionComplete();
            } // if
        } // DoMaterialize()

        /// <summary>
        /// Dissolve the spawn
        /// </summary>
        void Dissolve()
        {
            var i = 0;
            foreach (var material in _materials)
            {
                if (i == _materials.Count - 1) // if on last material to dissolve
                {
                    StartCoroutine(DoDissolve(material.Key, _dissolveKeyword, _dissolveLength, true));
                }
                else
                {
                    StartCoroutine(DoDissolve(material.Key, _dissolveKeyword, _dissolveLength));
                }
                ++i;
            } // for i
        } // Dissolve()

        /// <summary>
        /// Dissolve the spawn's materials
        /// </summary>
        /// <param name="material"></param>
        /// <param name="dissolveKeyword"></param>
        /// <param name="duration"></param>
        /// <param name="callCompleteHandler"></param>
        /// <returns></returns>
        IEnumerator DoDissolve(Material material, string dissolveKeyword, float duration, bool callCompleteHandler = false)
        {
            yield return new WaitForSeconds(_dissolveDelay);

            float timeElapsed = 0;
            float dissolve;

            while (timeElapsed < duration)
            {
                dissolve = Mathf.Lerp(0f, 1f, timeElapsed / duration);
                material.SetFloat(dissolveKeyword, dissolve);
                timeElapsed += Time.deltaTime;

                yield return null;
            }

            // Snap to final value
            dissolve = 1f;
            material.SetFloat(dissolveKeyword, dissolve);

            if (callCompleteHandler)
            {
                DespawnActionComplete();
            } // if
        } // DoDissolve()

        override public void SpawnActionComplete()
        {
            foreach (var material in _materials)
            {
                Material origMaterial = material.Value;
                material.Key.CopyPropertiesFromMaterial(origMaterial);
            } // foreach

            base.SpawnActionComplete();
        } // SpawnActionComplete()

        override public void FinalSpawnState()
        {
            SetVisible(_renderers, _materials);
        } // FinalSpawnState()

        override public void FinalDespawnState()
        {
            // nothing required at this time
        } // FinalDespawnState()

        override public float GetSpawnActionDuration()
        {
            return _materializeLength;
        } // GetSpawnActionDuration()

        override public float GetDespawnActionDuration()
        {
            return _dissolveLength;
        } // GetDespawnActionDuration()
    } // class SpawnAction_Fader
} // namespace Magique.SoulLink
