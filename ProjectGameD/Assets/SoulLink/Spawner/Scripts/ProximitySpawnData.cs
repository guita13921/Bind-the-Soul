// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

using System;

namespace Magique.SoulLink
{
    [System.Serializable]
    public class ProximitySpawnData
    {
        public float minRange = 5f;
        public float maxRange = 30f;

        public SpawnInfoData spawnInfoData;
    } // class ProximitySpawnData
} // namespace Magique.SoulLink

