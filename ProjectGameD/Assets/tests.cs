using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tests : MonoBehaviour
{   

    public float damage;
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            print("HIT");


        var enemy = other.gameObject.GetComponent<enemy>();
        if(enemy != null)
        {
            enemy.health.currentHealth -= damage;

            if(enemy.health.currentHealth <= 0)
            {
                Destroy(enemy.gameObject);
            }
        }







        }

}
}
