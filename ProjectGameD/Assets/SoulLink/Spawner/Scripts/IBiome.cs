using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2021 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface IBiome
    {
        bool IsInBiome(Vector3 spawnPos);
    } // interface IBiome
} // namespace Magique.SoulLink
