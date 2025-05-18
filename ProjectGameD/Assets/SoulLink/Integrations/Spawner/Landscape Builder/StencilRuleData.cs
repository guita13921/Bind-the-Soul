#if LANDSCAPE_BUILDER
using static Magique.SoulLink.SoulLinkSpawnerTypes;
using LandscapeBuilder;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique
{
    [System.Serializable]
    public class StencilRuleData : object
    {
        public LBStencil stencil;
        public LBStencilLayer layer;
        public FilterRule stencilRule;
        public float threshold = 0.5f;
    } // class StencilRuleData
} // namespace Magique
#endif