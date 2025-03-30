using UnityEngine;

namespace EnemyAI
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int Health = 1000;

        public void OnDamage(int damage)
        {
            Health -= damage;
        }
    }
}