
// ISpawnEvents
// An interface for notifying an object if it has spawned or despawned

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISpawnEvents
    {
        void OnSpawned();

        void OnDespawned();
    } // interface ISpawnEvents
} // namespace Magique.Spawnable