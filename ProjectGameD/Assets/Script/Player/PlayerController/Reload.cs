
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl
{   


    private void Reload()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(currentSceneIndex);
    }

}
