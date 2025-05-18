using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISoulLinkGuid
    {
        void GenerateGuid();
        string GetGuid();
        void SetGuid(string value);
    } // interface ISoulLinkGuid
} // namespace Magique.SoulLink
