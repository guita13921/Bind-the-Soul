
// Interface for spawn actions that can be performed on spawn or despawn

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISpawnAction
    {
        void PerformSpawnAction();

        void PerformDespawnAction();

        void SpawnActionComplete();

        void DespawnActionComplete();

        void FinalSpawnState();

        void FinalDespawnState();

        float GetSpawnActionDuration();

        float GetDespawnActionDuration();

        bool IsPerforming();
    } // interface ISpawnAction
} // namespace Magique.SoulLink