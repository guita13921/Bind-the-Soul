using System.Collections;
using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    public GameObject interactText; // UI Text that says "Press F to Read"
    public GameObject messageCanvas; // Canvas with the full message
    private bool isPlayerInRange = false; // Track if player is in the trigger zone

    private void Start()
    {
        interactText.gameObject.SetActive(false); // Hide "Press F to Read" text
        messageCanvas.SetActive(false); // Hide the full message canvas
    }

    private void Update()
    {
        // Only listen for 'F' press if player is within the collider
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Toggle the full message canvas
            messageCanvas.SetActive(!messageCanvas.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if it's the player
        {
            interactText.gameObject.SetActive(true); // Show "Press F to Read"
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactText.gameObject.SetActive(false); // Hide "Press F to Read"
            messageCanvas.SetActive(false); // Hide the full message
            isPlayerInRange = false;
        }
    }
}
