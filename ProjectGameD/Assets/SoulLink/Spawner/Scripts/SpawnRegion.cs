using UnityEngine;
using UnityEngine.Events;

// SpawnArea
// This component is used to define large regions that can have different spawner databases.
//
// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Spawn Region")]
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnRegion : MonoBehaviour
    {
        [TooltipAttribute("Select the Tag that will trigger a region change.")]
        [SerializeField]
        private string _triggerTag = "Player";

        public SoulLinkSpawnerDatabase Database
        { get { return _database; } }

        [SerializeField]
        private SoulLinkSpawnerDatabase _database;

        [SerializeField]
        private UnityEvent _onTriggeredEventHandler;

        void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        } // Awake()

        private void OnTriggerEnter(Collider other)
        {
            // Switch database gracefully
            if (other.CompareTag(_triggerTag))
            {
                // Call user's custom triggered event handler
                if (_onTriggeredEventHandler != null)
                {
                    _onTriggeredEventHandler.Invoke();
                } // if

                SoulLinkSpawner.Instance.SwitchDatabase(_database);
            } // if
        } // OnTriggerEnter()
    } // class SpawnRegion
} // namespace Magique.SoulLink
