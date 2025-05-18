using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveTestControl : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!GetComponent<WaveSpawner>().IsPaused)
                    GetComponent<WaveSpawner>().PauseWave();
                else
                    GetComponent<WaveSpawner>().UnPauseWave();
            } // if
        } // Update()
    } // class WaveTestControl
} // namespace Magique.SoulLink
