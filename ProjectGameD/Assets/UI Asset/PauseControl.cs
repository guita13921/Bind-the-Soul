using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioSource backgroundMusic;
    bool active = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!active)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f; // Pause the game
                backgroundMusic.enabled = false;
                active = true;
            }
            else
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f; // Pause the game
                backgroundMusic.enabled = true; // Pause music
                active = false;
            }
        }

    }

    public void settimescale()
    {
        Time.timeScale = 1f; // Pause the game
        backgroundMusic.enabled = true; // Pause music
    }
}
