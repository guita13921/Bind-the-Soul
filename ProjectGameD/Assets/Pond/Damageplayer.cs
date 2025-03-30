using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class DamagePlayer : MonoBehaviour
    {
        public int damage = 25;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("✅ OnTriggerEnter ทำงาน: " + other.gameObject.name); // เช็คว่ามีการชนเกิดขึ้น

            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                Debug.Log("✅ PlayerStats พบแล้ว! กำลังลด HP...");
                playerStats.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("❌ ไม่พบ PlayerStats บน: " + other.gameObject.name);
            }
        }
    }
}
