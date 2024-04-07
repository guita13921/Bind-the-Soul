using UnityEngine;

namespace EnemyAI.Sensors
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerSensor : MonoBehaviour
    {
        public SphereCollider Collider;
        public delegate void PlayerEnterEvent(Transform player);
        public delegate void PlayerExitEvent(Vector3 lastKnownPosition);

        public event PlayerEnterEvent OnPlayerEnter;
        public event PlayerExitEvent OnPlayerExit;

        private void Awake()
        {
            Collider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("IN");
            if (other.TryGetComponent(out PlayerControl Player))
            {
                OnPlayerEnter?.Invoke(Player.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("OUT");
            if (other.TryGetComponent(out PlayerControl Player))
            {
                OnPlayerExit?.Invoke(other.transform.position);
            }
        }
    }
}