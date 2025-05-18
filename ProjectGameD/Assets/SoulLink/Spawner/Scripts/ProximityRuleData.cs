using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class ProximityRuleData
    {
        public Transform spawnPrefab;
        public FilterRule proximityRule;
        public float range = 15f;

        public void CopyFrom(ProximityRuleData source)
        {
            spawnPrefab = source.spawnPrefab;
            proximityRule = source.proximityRule;
            range = source.range;
        } // CopyFrom()
    } // class ProximityRuleData
} // namespace Magique.SoulLink

