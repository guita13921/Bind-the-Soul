using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
    public class CharacterStats : MonoBehaviour
    {
        public int healthLevel;
        public int maxHealth;
        public int currentHealth;

        public int goldCount = 0;

        public bool isDead;

        public virtual void TakeDamage(int damage, string damageAnimation = "Danage_01")
        {

        }

    }
}