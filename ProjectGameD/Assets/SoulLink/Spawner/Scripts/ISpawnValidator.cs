// Copyright 2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

using static Magique.SoulLink.SoulLinkSpawnerTypes;

namespace Magique.SoulLink
{
    public interface ISpawnValidator
    {
        bool PreValidateSpawn(SpawnInfoData spawnInfoData);

        bool PostValidateSpawn(SpawnInfoData spawnInfoData, SpawnValidationFailure failureReason);
    } // ISpawnValidator
} // namespace Magique.SoulLink
