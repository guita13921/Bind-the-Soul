using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BaseSpline : MonoBehaviour, ISpline
    {
        virtual public float GetLength()
        {
            throw new System.NotImplementedException();
        }

        virtual public Vector3 GetWorldPosition(float distance)
        {
            throw new System.NotImplementedException();
        }
    } // class BaseSpline
} // namespace Magique.SoulLink