using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PlayerSpawn : MonoBehaviour
    {
        /// <summary>
        /// Call this method when the player dies so that it can be respawned
        /// </summary>
        public void OnPlayerDeath()
        {
            PlayerSpawner.Instance.Respawn();
        } // OnPlayerDeath()
    } // class PlayerSpawn
} // namespace Magique.SoulLink
