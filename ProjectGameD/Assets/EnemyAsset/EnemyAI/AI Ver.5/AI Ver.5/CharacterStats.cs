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

        public int staminaLevel = 10;
        public float maxStamina;
        public float currentStamina;

        public bool isDead;

    }
}