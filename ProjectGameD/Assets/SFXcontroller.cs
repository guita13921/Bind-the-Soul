using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXcontroller : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    float waitforspawn = 0f;

    public void Start()
    {
        if (audioSource != null)
        {
            StartCoroutine(PlaySFXAfterDelay());
        }
    }

    private IEnumerator PlaySFXAfterDelay()
    {
        yield return new WaitForSeconds(waitforspawn);
        audioSource.Play();
    }
}
