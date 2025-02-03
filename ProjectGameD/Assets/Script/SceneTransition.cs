using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public TransitionSettings transition;
    public float loaddelay;

    public void NextScene(string _sceneName)
    {
        PlayTransitionSound();
        if (_sceneName == "MainMenu")
            Time.timeScale = 1f;
        TransitionManager.Instance().Transition(_sceneName, transition, loaddelay);
    }

    [SerializeField]
    private AudioClip Transitionsound;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioSource currentSoundtrack;

    public void PlayTransitionSound()
    {
        if (currentSoundtrack != null)
        {
            currentSoundtrack.volume = 0f;
        }
        if (audioSource != null && Transitionsound != null)
        {
            audioSource.PlayOneShot(Transitionsound); // Play the sound instantly
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }
}
