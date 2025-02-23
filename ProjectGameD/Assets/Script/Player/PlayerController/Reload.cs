using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl
{
    private void Reload()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);
    }
}
