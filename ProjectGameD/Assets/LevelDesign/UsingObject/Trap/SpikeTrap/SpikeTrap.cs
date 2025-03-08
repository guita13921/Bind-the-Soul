using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField]
    private GameObject spikes; // The spike object

    [SerializeField]
    private float activationDelay; // Delay before spikes activate

    [SerializeField]
    private float resetDelay; // Delay before resetting the trap

    [SerializeField]
    private float spikeHeight; // Height the spikes shoot up

    [SerializeField]
    private float spikeSpeed; // Speed at which the spikes shoot up

    [SerializeField]
    private bool autoReset = true; // If true, the trap resets automatically
    public PlayerCombat playerCombat;
    public CharacterData characterData;
    private float reducedDamageSecond = 0; // if HP < 25% of maxHP

    [Header("Trap Settings")]
    [SerializeField] private float damage;
    [SerializeField] private Health player;

    private Vector3 initialPosition; // Original position of the spikes
    private Vector3 activePosition; // Position when spikes are fully activated
    private bool isTriggered = false; // Prevent multiple triggers

    [SerializeField] private bool isActive = false; // Whether the spikes are currently active
    PlayerControl playerControl;
    private void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        playerCombat = FindObjectOfType<PlayerCombat>();
        initialPosition = spikes.transform.localPosition;
        activePosition = initialPosition + new Vector3(0, spikeHeight, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the trap is triggered
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(ActivateTrap());
        }

        // If spikes are active, calculate damage
        if (isActive)
        {
            ApplyDamage(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive)
        {
            ApplyDamage(other);
            isActive = false;
        }
    }

    private IEnumerator ActivateTrap()
    {
        // Wait for the activation delay
        yield return new WaitForSeconds(activationDelay);

        // Move spikes up
        yield return StartCoroutine(MoveSpikes(activePosition));

        // Spikes are now active
        isActive = true;

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
            spikes.transform.localPosition = Vector3.MoveTowards(
                spikes.transform.localPosition,
                targetPosition,
                spikeSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void ResetTrap()
    {
        // Move spikes back to the initial position
        StartCoroutine(MoveSpikes(initialPosition));
        isTriggered = false;
        isActive = false; // Spikes are no longer active
    }

    private void ApplyDamage(Collider other)
    {
     
        //Debug.Log("ApplyDamage");
        player = other.gameObject.GetComponent<Health>();
           if(playerControl != null){
            playerControl.GetHit();
        }
        if (player != null && other.CompareTag("Player"))
        {
            if (!playerCombat.isShield1 && !playerCombat.isShield2)
            {
                if (player.currentHealth < (player.maxHealth * 0.25f))
                {
                    reducedDamageSecond = characterData.reduceIncomeDamageDependOnHP * 0.15f; // 0.15f per level (15%, 30%, 45%)
                }
                float damageReductionPercentage = characterData.reduceIncomeDamage * 0.05f; // 0.05f per level (5%, 10%, 15%)
                float reducedDamage = damage * damageReductionPercentage;
                float reducedDamageDependOnHP = damage * reducedDamageSecond;
                player.currentHealth -= Mathf.Max(
                    0,
                    damage - reducedDamage - reducedDamageDependOnHP
                );
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Optional: Visualize the trigger area in the editor
        Gizmos.color = Color.red;
        var boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
    }
}
