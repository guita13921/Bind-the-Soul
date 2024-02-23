using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<enemy>();
        if (enemy != null)
        {
            enemy.health.currentHealth -= damage;
            print("Enemy hit");

            if (enemy.health.currentHealth <= 0)
            {
                if(enemy.gameObject.CompareTag("Player")){

                }else{
                //Destroy(enemy.gameObject);

                }
            }
        }
    }
}
