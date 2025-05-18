using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class BiomeSpawnData
    {
        public string biomeName;

        public TimeOfDaySpawnsDictionary timeOfDaySpawns = new TimeOfDaySpawnsDictionary();

        public bool disabled = false;
    } // class BiomeSpawnData
} // namespace Magique.SoulLink

