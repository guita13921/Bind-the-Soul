using System.Collections;
using UnityEngine;

public class HS_ZoneTrigger : MonoBehaviour
{
    public ParticleSystem stableEffect;
    public GameObject triggerEffect;
    public float repeatTime = 20;
    private bool canRepeat = true;

    private AudioClip clip;
    private AudioSource soundComponent;

    public float moveDuration = 2f; // Time to move from y = -1 to y = 0

    void Start()
    {
        // Set initial position
        Vector3 startPosition = transform.position;
        startPosition.y = -1;
        transform.position = startPosition;

        // Smoothly move to y = 0
        StartCoroutine(MoveUpwards());

        // Initialize audio
        if (soundComponent == null)
        {
            soundComponent = GetComponent<AudioSource>();
            if (soundComponent != null)
            {
                clip = soundComponent.clip;
            }
        }

        stableEffect.Play();
    }

    IEnumerator MoveUpwards()
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, 0, startPos.z);

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Ensure final position is correct
    }

    void OnTriggerEnter(Collider other)
    {
        if (canRepeat) // Only triggers if Player enters
        {
            canRepeat = false;
            stableEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Instantiate(triggerEffect, this.transform.position, this.transform.rotation);

            if (soundComponent != null && clip != null)
            {
                soundComponent.PlayOneShot(clip);
            }

            StartCoroutine(ResetTrigger());
        }
    }

    IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(repeatTime);
        canRepeat = true;

        // Restart stable effect
        stableEffect.Play();
    }
}
