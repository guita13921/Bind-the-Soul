using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// Copyright 2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnValidator : MonoBehaviour, ISpawnValidator
    {
        virtual public bool PreValidateSpawn(SpawnInfoData spawnInfoData)
        {
            // Succeed by default
            return true;
        } // PreValidateSpawn()

        virtual public bool PostValidateSpawn(SpawnInfoData spawnInfoData, SpawnValidationFailure failureReason)
        {
            // Fail by default
            return false;
        } // PostValidateSpawn()
    } // class SpawnValidator
} // namespace Magique.SoulLink
