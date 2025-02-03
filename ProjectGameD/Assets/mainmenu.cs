using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainmenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check if Esc key is pressed
        {
            Time.timeScale = 1; // Set time scale back to normal speed

            gameObject.SetActive(false); // Disable the GameObject
        }
    }
}
