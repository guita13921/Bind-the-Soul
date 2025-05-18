using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Magique.SoulLink
{
    public static class ShaderUtils
    {
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        /// <summary>
        /// ChangeRenderMode
        /// </summary>
        /// <param name="shaderMaterial"></param>
        /// <param name="blendMode"></param>
        public static void ChangeRenderMode(Material shaderMaterial, BlendMode blendMode)
        {
            if (GraphicsSettings.currentRenderPipeline != null)
            {
                var srpType = GraphicsSettings.currentRenderPipeline.GetType().ToString();
                if (srpType.Contains("HDRenderPipelineAsset"))
                {
                    ChangeRenderModeHDRP(shaderMaterial, blendMode); ;
                }
                else if (srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset"))
                {
                    ChangeRenderModeURP(shaderMaterial, blendMode); ;
                }
                else
                {
                    Debug.LogError("ShaderUtils Render pipeline " + srpType + " is unsupported");
                }
            }
            else
            {
                // Built-in pipeline
                ChangeRenderModeStandard(shaderMaterial, blendMode);
            }
        } // ChangeRenderMode()

        /// <summary>
        /// ChangeRenderModeStandard
        /// </summary>
        /// <param name="shaderMaterial"></param>
        /// <param name="blendMode"></param>
        public static void ChangeRenderModeStandard(Material shaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    shaderMaterial.SetOverrideTag("RenderType", "");
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    shaderMaterial.SetInt("_ZWrite", 1);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.Geometry;
                    break;
                case BlendMode.Cutout:
                    shaderMaterial.SetOverrideTag("RenderType", "Transparent");
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    shaderMaterial.SetInt("_ZWrite", 1);
                    shaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    shaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.AlphaTest;
                    break;
                case BlendMode.Fade:
                    shaderMaterial.SetOverrideTag("RenderType", "Transparent");
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    shaderMaterial.SetInt("_ZWrite", 1);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.Transparent;
                    break;
                case BlendMode.Transparent:
                    shaderMaterial.SetOverrideTag("RenderType", "Transparent");
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    shaderMaterial.SetInt("_ZWrite", 0);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.Transparent;
                    break;
            } //  switch (blendMode)
        } // ChangeRenderModeStandard()

        /// <summary>
        /// ChangeRenderModeURP
        /// </summary>
        /// <param name="shaderMaterial"></param>
        /// <param name="blendMode"></param>
        public static void ChangeRenderModeURP(Material shaderMaterial, BlendMode blendMode)
        {
            bool toonyColorsPro = shaderMaterial.shader.name.Equals("Toony Colors Pro 2/Hybrid Shader");
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    shaderMaterial.SetFloat("_Surface", 0);
                    shaderMaterial.SetFloat("_RenderingMode", 0);
                    shaderMaterial.SetInt("_RenderQueueType", 1);
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    if (toonyColorsPro)
                    {
                        shaderMaterial.SetInt("_ZWrite", 1);
                        shaderMaterial.SetOverrideTag("RenderType", "");
                    }
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.Geometry;
                    break;
                case BlendMode.Transparent:
                    shaderMaterial.SetFloat("_Surface", 1);
                    shaderMaterial.SetFloat("_RenderingMode", 2);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    if (toonyColorsPro)
                    {
                        shaderMaterial.SetOverrideTag("RenderType", "Transparent");
                        shaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        shaderMaterial.SetInt("_ZWrite", 0);
                    }
                    else
                    {
                        shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        shaderMaterial.SetInt("_ZWrite", 1);
                        shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    }
                    shaderMaterial.renderQueue = (int)RenderQueue.Transparent;
                    break;
            } // switch
        } // ChangeRenderModeURP()

        /// <summary>
        /// ChangeRenderModeHDRP
        /// </summary>
        /// <param name="shaderMaterial"></param>
        /// <param name="blendMode"></param>
        public static void ChangeRenderModeHDRP(Material shaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    shaderMaterial.SetFloat("_SurfaceType", 0);
                    shaderMaterial.SetFloat("_ZTestDepthEqualForOpaque", 3);
                    shaderMaterial.SetInt("_RenderQueueType", 1);
                    shaderMaterial.SetOverrideTag("RenderType", "");
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    shaderMaterial.SetInt("_ZWrite", 1);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.Geometry;
                    break;
                case BlendMode.Transparent:
                    shaderMaterial.SetFloat("_SurfaceType", 1);
                    shaderMaterial.SetFloat("_ZTestDepthEqualForOpaque", 4);
                    shaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    shaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    shaderMaterial.SetInt("_ZWrite", 0);
                    shaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    shaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    shaderMaterial.SetOverrideTag("RenderType", "Transparent");
                    shaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    shaderMaterial.renderQueue = (int)RenderQueue.Transparent;
                    break;
            } // switch
        } // ChangeRenderModeHDRP()

        /// <summary>
        /// SetInvisible
        /// </summary>
        /// <param name="renderers"></param>
        /// <param name="materials"></param>
        /// <param name="texColorName"></param>
        public static void SetInvisible(List<Renderer> renderers, Dictionary<Material, Material> materials, string texColorName)
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            } // foreach

            foreach (var material in materials)
            {
                ChangeRenderMode(material.Key, BlendMode.Transparent);
                Color color = material.Key.GetColor(texColorName);
                material.Key.SetColor(texColorName, new Color(color.r, color.g, color.b, 0f));
            } // for i
        } // SetInvisible()

        /// <summary>
        /// SetVisible
        /// </summary>
        /// <param name="renderers"></param>
        /// <param name="materials"></param>
        public static void SetVisible(List<Renderer> renderers, Dictionary<Material, Material> materials)
        {
            foreach (var material in materials)
            {
                Material origMaterial = new Material(material.Value);
                material.Key.CopyPropertiesFromMaterial(origMaterial);
            } // for i

            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            } // foreach
        } // SetVisible()
    } // class class ShaderUtils
} // namespace Magique.SoulLink