using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Enemy : MonoBehaviour
{
     public float damage;
    public SFX sfx;

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
                if(enemy.gameObject.CompareTag("Player")){
                sfx.Hit();

            enemy.health.currentHealth -= damage;}
        
            
        }
    }
}
