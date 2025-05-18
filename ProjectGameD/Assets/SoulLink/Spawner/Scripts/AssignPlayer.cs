using UnityEngine;
using UnityEngine.SceneManagement;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Assign Player")]
    public class AssignPlayer : MonoBehaviour
    {
        /// <summary>
        /// Assign the player's transform to SoulLinkSpawner when the player starts in the scene
        /// Update reference if a new scene is loaded, which may have a different SoulLinkSpawner component.
        /// </summary>
        void Start()
        {
            SetPlayerReference();
            SceneManager.sceneLoaded += OnSceneLoaded;
        } // Start()

        void SetPlayerReference()
        {
            if (SoulLinkSpawner.Instance != null)
            {
                SoulLinkSpawner.Instance.PlayerTransform = gameObject.transform;
            }
            else
            {
                Debug.LogWarning("AssignPlayer: SoulLink is not present in the scene. You may ignore this if this is your intention.");
            }
        } // SetPlayerReference()

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetPlayerReference();
        } // OnSceneLoaded()
    } // class AssignPlayer
} // namespace Magique.SoulLink
