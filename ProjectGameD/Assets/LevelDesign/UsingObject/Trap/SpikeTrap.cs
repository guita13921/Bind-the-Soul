using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private GameObject spikes; // The spike object
    [SerializeField] private float activationDelay; // Delay before spikes activate
    [SerializeField] private float resetDelay; // Delay before resetting the trap
    [SerializeField] private float spikeHeight; // Height the spikes shoot up
    [SerializeField] private float spikeSpeed; // Speed at which the spikes shoot up
    [SerializeField] private bool autoReset = true; // If true, the trap resets automatically

    private Vector3 initialPosition; // Original position of the spikes
    private Vector3 activePosition; // Position when spikes are fully activated
    private bool isTriggered = false; // Prevent multiple triggers

    private void Start()
    {
        // Store the initial and active positions
        initialPosition = spikes.transform.localPosition;
        activePosition = initialPosition + new Vector3(0, spikeHeight, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player steps on the trap
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(ActivateTrap());
        }
    }

    private IEnumerator ActivateTrap()
    {
        // Wait for the activation delay
        yield return new WaitForSeconds(activationDelay);

        // Move spikes up
        yield return StartCoroutine(MoveSpikes(activePosition));

        // Wait for the reset delay if auto-reset is enabled
        if (autoReset)
        {
            yield return new WaitForSeconds(resetDelay);
            ResetTrap();
        }
    }

    private IEnumerator MoveSpikes(Vector3 targetPosition)
    {
        // Smoothly move spikes to the target position
        while (Vector3.Distance(spikes.transform.localPosition, targetPosition) > 0.01f)
        {
            spikes.transform.localPosition = Vector3.MoveTowards(spikes.transform.localPosition, targetPosition, spikeSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void ResetTrap()
    {
        // Move spikes back to the initial position
        StartCoroutine(MoveSpikes(initialPosition));
        isTriggered = false;
    }

    private void OnDrawGizmos()
    {
        // Optional: Visualize the trigger area in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}
