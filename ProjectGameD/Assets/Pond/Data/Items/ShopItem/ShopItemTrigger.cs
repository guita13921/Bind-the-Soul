using UnityEngine;

namespace SG
{
    public class ShopItemTrigger : MonoBehaviour
    {
        public ShopItem parentShopItem;
        public PowerUp powerUp;
        public int cost;

        void Start()
        {
            cost = parentShopItem.cost;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogWarning("PlayerStats not found on Player.");
                return;
            }

            if (playerStats.goldCount >= cost)
            {
                playerStats.goldCount -= cost;

                if (powerUp != null)
                {
                    powerUp.Apply(playerStats);
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Not enough gold!");
            }
        }

    }
}
