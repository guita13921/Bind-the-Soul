using UnityEngine;

namespace Magique.SoulLink
{
    [CreateAssetMenu(fileName = "SpawnableSettings", menuName = "SoulLink/Spawner/Spawnable Settings", order = 3)]
    public class SpawnableSettingsObject : ScriptableObject
    {
        public string spawnAction = "SpawnAction_Fader";
        public float despawnDelay = 5f;
        public float despawnCheckInterval = 5f;
        public float despawnRangeBuffer = 15f;
        public bool requiresNavMesh = true;
        public bool isPersistent = false;
        public bool ignoreTotalSpawns = false;
    } // SpawnableSettingsObject
} // namespace Magique.SoulLink
