using UnityEngine;

namespace SG
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private Drop coinDrop;
        [SerializeField] public int coinValue;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CharacterStats characterStats = other.GetComponent<CharacterStats>();

                if (coinDrop.audioSource != null && coinDrop.audioSource.clip != null)
                {
                    AudioSource.PlayClipAtPoint(coinDrop.audioSource.clip, transform.position);
                }

                characterStats.goldCount += coinValue;
                Destroy(gameObject);
            }
        }
    }
}
