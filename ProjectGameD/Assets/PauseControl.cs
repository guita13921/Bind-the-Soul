using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioSource backgroundMusic;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f; // Pause the game
            backgroundMusic.enabled = false; // Pause music
        }
    }

    public void settimescale()
    {
        Time.timeScale = 1f; // Pause the game
        backgroundMusic.enabled = true; // Pause music
    }
}
