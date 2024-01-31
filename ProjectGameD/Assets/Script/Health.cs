using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    void Start(){
        currentHealth = maxHealth;
    }


     void Update() {
        if(currentHealth<=0){
            Destroy(gameObject);
        }
        
        
    }

    
}
