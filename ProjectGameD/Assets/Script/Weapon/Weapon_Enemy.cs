using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Enemy : MonoBehaviour
{
     public float damage;

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
                if(enemy.gameObject.CompareTag("Player"))
            enemy.health.currentHealth -= damage;
        
            
        }
    }
}
