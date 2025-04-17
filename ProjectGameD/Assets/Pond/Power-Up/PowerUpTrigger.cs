using UnityEngine;

namespace SG
{

    public class PowerUpTrigger : MonoBehaviour
    {
        public PowerUp powerUp; // Assign in Inspector
        public AudioClip pickupSound; // Optional
        public GameObject pickupVFX;  // Optional

        private bool isCollected = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            if (other.CompareTag("Player"))
            {
                isCollected = true;

                PowerUpManager manager = FindObjectOfType<PowerUpManager>();

                if (manager != null && powerUp != null)
                {
                    manager.AddPowerUp(powerUp);

                    if (pickupSound)
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);

                    if (pickupVFX)
                        Instantiate(pickupVFX, transform.position, Quaternion.identity);

                    Debug.Log($"Collected power-up: {powerUp.Name}");

                    Destroy(gameObject); // Remove the power-up from the scene
                }
            }
        }
    }
}