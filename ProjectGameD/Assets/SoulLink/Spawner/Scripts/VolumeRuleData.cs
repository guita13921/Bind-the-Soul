using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class VolumeRuleData
    {
        public FilterRule filterRule;
        public Collider volume;
        public float depth = 0f;
    } // class VolumeRuleData
} // namespace Magique.SoulLink

