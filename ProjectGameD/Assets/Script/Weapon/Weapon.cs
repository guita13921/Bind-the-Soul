using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   
    public float damage;
    BoxCollider triggerBox;

    // Time to enable the trigger after the "J" key is pressed.
    float triggerEnableTime = 0.5f;
    float currentTriggerEnableTime = 0f;

    void Start()
    {
        triggerBox = GetComponent<BoxCollider>();
        triggerBox.isTrigger = false; // Initially disable the trigger.
    }

    void Update()
    {
        // Check for input and enable the trigger accordingly.
        HandleInput();

        // Update the timer if the trigger is enabled.
        if (triggerBox.isTrigger)
        {
            currentTriggerEnableTime -= Time.deltaTime;

            // Disable the trigger after the specified time.
            if (currentTriggerEnableTime <= 0)
            {
                triggerBox.isTrigger = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other);
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

    void HandleInput()
    {
        // Check if the "J" key is pressed.
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Enable the trigger and set the timer.
            triggerBox.isTrigger = true;
            currentTriggerEnableTime = triggerEnableTime;
        }
    }
}
