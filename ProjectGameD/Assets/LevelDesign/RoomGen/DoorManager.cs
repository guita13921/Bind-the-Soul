using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Components")]
    public GameObject doorPanelLeft;    // First part of the door
    public GameObject doorPanelRight;   // Second part of the door
    public Light pointLight;            // The point light to enable on open

    public void OpenDoor()
    {
        // Deactivate both door panel parts if they exist
        if (doorPanelLeft != null)
        {
            doorPanelLeft.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DoorManager: Left door panel not assigned!");
        }

        if (doorPanelRight != null)
        {
            doorPanelRight.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DoorManager: Right door panel not assigned!");
        }

        // Enable the point light if assigned
        if (pointLight != null)
        {
            pointLight.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("DoorManager: Point light not assigned!");
        }
    }

    public void CloseDoor()
    {
        // Reactivate both door panel parts if they exist
        if (doorPanelLeft != null)
        {
            doorPanelLeft.SetActive(true);
        }
        else
        {
            Debug.LogWarning("DoorManager: Left door panel not assigned!");
        }

        if (doorPanelRight != null)
        {
            doorPanelRight.SetActive(true);
        }
        else
        {
            Debug.LogWarning("DoorManager: Right door panel not assigned!");
        }

        // Disable the point light if assigned
        if (pointLight != null)
        {
            pointLight.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DoorManager: Point light not assigned!");
        }
    }
}
