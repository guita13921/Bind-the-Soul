using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISpline
    {
        float GetLength();

        Vector3 GetWorldPosition(float distance);
    } // interface ISpline
} // namespace Magique.SoulLink