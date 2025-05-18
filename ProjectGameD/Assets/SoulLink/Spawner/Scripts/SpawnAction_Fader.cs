using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.ShaderUtils;

// SpawnAction_Fader
// A built-in spawn action for fading spawns in/out
//
// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnAction_Fader : BaseSpawnAction
    {
        public string FadeTexColorName
        {
            get { return _texColorName; }
            set { _texColorName = value; }
        }

        [Tooltip("For Standard Shader use _Color and for URP Lit Shader use _BaseColor.")]
        [SerializeField]
        protected string _texColorName = "_Color";

        [Tooltip("How long to take for the spawn to fully fade in.")]
        [SerializeField]
        protected float _fadeInLength = 2f;

        [Tooltip("How long to take for the spawn to fully fade out.")]
        [SerializeField]
        protected float _fadeOutLength = 2f;

        protected Dictionary<Material, Material> _materials = new Dictionary<Material, Material>();
        protected List<Renderer> _renderers = new List<Renderer>();

        /// <summary>
        /// Prepare this object for fading in/out
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
                        if (material.HasProperty(_texColorName))
                        {
                            _materials[material] = new Material(material);
                        } // if
                    } // foreach material
                } // if
            } // foreach renderer

            SetInvisible(_renderers, _materials, _texColorName);
        } // Awake()

        public void SetTexColorName(string value)
        {
            _texColorName = value;
        }

        public string GetTexColorName()
        {
            return _texColorName;
        }

        public void SetFadeInLength(float value)
        {
            _fadeInLength = value;
        } // SetFadeInLength()

        public float GetFadeInLength()
        {
            return _fadeInLength;
        } // GetFadeInLength()

        public void SetFadeOutLength(float value)
        {
            _fadeOutLength = value;
        } // SetFadeOutLength()

        public float GetFadeOutLength()
        {
            return _fadeOutLength;
        } // GetFadeOutLength()

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

                FadeIn();
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

                FadeOut();
            }
            else
            {
                DespawnActionComplete();
            }
        } // PerformDespawnAction()

        /// <summary>
        /// Fade in the spawn
        /// </summary>
        void FadeIn()
        {
            foreach (var renderer in _renderers)
            {
                renderer.enabled = true;
            }

            var i = 0;
            foreach (var material in _materials)
            {
                // Start alpha value at 0
                ChangeRenderMode(material.Key, BlendMode.Transparent);
                Color color = material.Key.GetColor(_texColorName);
                material.Key.SetColor(_texColorName, new Color(color.r, color.g, color.b, 0f));

                if (i == _materials.Count - 1)
                {
                    StartCoroutine(DoFadeIn(material.Key, _texColorName, _fadeInLength, true));
                }
                else
                {
                    StartCoroutine(DoFadeIn(material.Key, _texColorName, _fadeInLength));
                }
                ++i;
            } // foreach
        } // FadeIn()

        /// <summary>
        /// Fade in the spawn's materials
        /// </summary>
        /// <param name="material"></param>
        /// <param name="texColorName"></param>
        /// <param name="duration"></param>
        /// <param name="callCompleteHandler"></param>
        /// <returns></returns>
        IEnumerator DoFadeIn(Material material, string texColorName, float duration, bool callCompleteHandler = false)
        {
            float timeElapsed = 0;
            float alpha;

            while (timeElapsed < duration)
            {
                alpha = Mathf.Lerp(0f, 1f, timeElapsed / duration);
                material.SetColor(texColorName, new Color(material.color.r, material.color.g, material.color.b, alpha));
                timeElapsed += Time.deltaTime;

                yield return null;
            } // while

            // Snap to final value
            alpha = 1f;
            material.SetColor(texColorName, new Color(material.color.r, material.color.g, material.color.b, alpha));

            if (callCompleteHandler)
            {
                SpawnActionComplete();
            } // if
        } // DoFadeIn()

        /// <summary>
        /// Fade out the spawn
        /// </summary>
        void FadeOut()
        {
            var i = 0;
            foreach (var material in _materials)
            {
                ChangeRenderMode(material.Key, BlendMode.Transparent);
                if (i == _materials.Count - 1) // if on last material to fade out
                {
                    StartCoroutine(DoFadeOut(material.Key, _texColorName, _fadeOutLength, true));
                }
                else
                {
                    StartCoroutine(DoFadeOut(material.Key, _texColorName, _fadeOutLength));
                }
                ++i;
            } // for i
        } // FadeOut()

        /// <summary>
        /// Fade out the spawn's materials
        /// </summary>
        /// <param name="material"></param>
        /// <param name="texColorName"></param>
        /// <param name="duration"></param>
        /// <param name="callCompleteHandler"></param>
        /// <returns></returns>
        IEnumerator DoFadeOut(Material material, string texColorName, float duration, bool callCompleteHandler = false)
        {
            float timeElapsed = 0;
            float alpha;

            while (timeElapsed < duration)
            {
                alpha = Mathf.Lerp(1f, 0f, timeElapsed / duration);
                material.SetColor(texColorName, new Color(material.color.r, material.color.g, material.color.b, alpha));
                timeElapsed += Time.deltaTime;

                yield return null;
            }

            // Snap to final value
            alpha = 0f;
            material.SetColor(texColorName, new Color(material.color.r, material.color.g, material.color.b, alpha));

            if (callCompleteHandler)
            {
                DespawnActionComplete();
            } // if
        } // DoFadeOut()

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
            return _fadeInLength;
        } // GetSpawnActionDuration()

        override public float GetDespawnActionDuration()
        {
            return _fadeOutLength;
        } // GetDespawnActionDuration()
    } // class SpawnAction_Fader
} // namespace Magique.SoulLink
