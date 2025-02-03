using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonSound; // Assign the audio clip in Inspector

    [SerializeField]
    private AudioSource audioSource; // Assign an AudioSource in Inspector

    public void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound); // Play the sound instantly
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }
}
