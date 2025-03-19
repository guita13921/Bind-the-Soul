using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [Header("Floor Trap Settings")]
    [SerializeField] private GameObject floorTrap; // The floor trap object
    [SerializeField] private float activationDelay = 0.5f; // Delay before turrets activate after stepping

    [Header("Arrow Turrets Settings")]
    [SerializeField] private List<Transform> turrets; // List of turrets in the level
    [SerializeField] private GameObject arrowPrefab; // The arrow prefab
    [SerializeField] private float arrowSpeed = 10f; // Speed of the arrow
    [SerializeField] private float fireRate = 1f; // Rate at which each turret fires arrows
    [SerializeField] private int arrowCount = 3; // Number of arrows fired by each turret after activation

    private bool isTriggered = false; // Prevent multiple activations

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player steps on the floor trap
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(ActivateTrap(other.transform.position));
        }
    }

    private IEnumerator ActivateTrap(Vector3 floorTrapPosition)
    {
        // Wait for the activation delay
        yield return new WaitForSeconds(activationDelay);

        // Activate all turrets to fire arrows at the floor trap's position
        foreach (Transform turret in turrets)
        {
            StartCoroutine(FireArrowsFromTurret(turret, floorTrapPosition));
        }

        // Reset the trap after all turrets finish firing
        yield return new WaitForSeconds((arrowCount / fireRate) + 0.5f);
        ResetTrap();
    }

    private IEnumerator FireArrowsFromTurret(Transform turret, Vector3 targetPosition)
    {
        for (int i = 0; i < arrowCount; i++)
        {
            ShootArrow(turret, targetPosition);
            yield return new WaitForSeconds(1f / fireRate); // Wait between shots based on fire rate
        }
    }

    private void ShootArrow(Transform turret, Vector3 targetPosition)
    {
        // Instantiate the arrow at the turret's position
        GameObject arrow = Instantiate(arrowPrefab, turret.position, Quaternion.identity);

        // Calculate the direction to the target while ignoring the Y-axis
        Vector3 direction = (targetPosition - turret.position);
        direction.y = 0; // Ignore Y-axis by setting it to 0
        direction = direction.normalized;

        // Introduce random error in the direction's Z-axis
        float error = Random.Range(-0.01f, 0.01f); // Adjust the range for more or less error
        direction = Quaternion.Euler(0, error * 360f, 0) * direction;

        // Set the arrow's rotation to face the (slightly adjusted) target direction
        arrow.transform.rotation = Quaternion.LookRotation(direction);

        // Apply velocity to the arrow to shoot it
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = false; // Ensure it's not kinematic
            arrowRb.velocity = direction * arrowSpeed; // Apply velocity in the adjusted direction
        }
        else
        {
            Debug.LogError("Arrow prefab is missing a Rigidbody component!");
        }

        // Optional: Destroy the arrow after a certain duration to prevent clutter
        Destroy(arrow, 5f);
    }

    private void ResetTrap()
    {
        isTriggered = false; // Reset the trap for future activations
    }

    private void OnDrawGizmos()
    {
        // Optional: Visualize the trigger area in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);

        // Visualize the turret positions
        Gizmos.color = Color.blue;
        foreach (Transform turret in turrets)
        {
            if (turret != null)
            {
                Gizmos.DrawLine(transform.position, turret.position);
            }
        }
    }
}
