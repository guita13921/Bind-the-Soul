using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Components")]
    public GameObject doorPanelLeft;    // First part of the door
    public GameObject doorPanelRight;   // Second part of the door
    public Light pointLight;            // The point light to enable on open

    public void OpenDoor()
    {
        // Destroy both door panel parts if they exist
        if (doorPanelLeft != null)
        {
            Destroy(doorPanelLeft);
        }
        else
        {
            Debug.LogWarning("DoorManager: Left door panel not assigned!");
        }

        if (doorPanelRight != null)
        {
            Destroy(doorPanelRight);
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
}
